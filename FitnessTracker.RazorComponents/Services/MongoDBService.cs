using System.Threading.Tasks;
using MongoDB.Driver;
using FitnessTracker.RazorComponents.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FitnessTracker.RazorComponents.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly string _connectionString;
        
        // Collections
        private readonly IMongoCollection<Exercise> _exercisesCollection;
        private readonly IMongoCollection<WorkoutLog> _workoutLogsCollection;
        private readonly IMongoCollection<UserProfile> _userProfilesCollection;

        public MongoDBService(IConfiguration configuration)
        {
            // Read connection string from configuration
            _connectionString = configuration["MongoDB:ConnectionString"] ?? throw new InvalidOperationException("MongoDB connection string not found in configuration");
            
            // Create a MongoClient with the connection string using direct connection format
            // Important: Use direct node connection with replica set format to avoid DNS SRV issues on Android
            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));
            
            // Set server connection timeout to handle mobile network issues
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            
            // Enable retry writes for better mobile reliability
            clientSettings.RetryWrites = true;
            
            var client = new MongoClient(clientSettings);
            
            // Get the database
            string databaseName = configuration["MongoDB:DatabaseName"] ?? "FitnessTracker";
            _database = client.GetDatabase(databaseName);
            
            // Initialize collections
            _exercisesCollection = _database.GetCollection<Exercise>("Exercises");
            _workoutLogsCollection = _database.GetCollection<WorkoutLog>("WorkoutLogs");
            _userProfilesCollection = _database.GetCollection<UserProfile>("UserProfiles");
        }
        
        // Exercise methods
        public async Task<List<Exercise>> GetExercisesAsync()
        {
            return await _exercisesCollection.Find(e => true).ToListAsync();
        }
        
        public async Task<Exercise> GetExerciseAsync(string id)
        {
            return await _exercisesCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task CreateExerciseAsync(Exercise exercise)
        {
            await _exercisesCollection.InsertOneAsync(exercise);
        }
        
        public async Task UpdateExerciseAsync(string id, Exercise exercise)
        {
            await _exercisesCollection.ReplaceOneAsync(e => e.Id == id, exercise);
        }
        
        public async Task DeleteExerciseAsync(string id)
        {
            await _exercisesCollection.DeleteOneAsync(e => e.Id == id);
        }
        
        // Workout Log methods
        public async Task<List<WorkoutLog>> GetWorkoutLogsAsync(string userId)
        {
            return await _workoutLogsCollection.Find(w => w.UserId == userId).ToListAsync();
        }
        
        public async Task<WorkoutLog> GetWorkoutLogAsync(string id)
        {
            return await _workoutLogsCollection.Find(w => w.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task<List<WorkoutLog>> GetWorkoutLogsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<WorkoutLog>.Filter.And(
                Builders<WorkoutLog>.Filter.Eq(w => w.UserId, userId),
                Builders<WorkoutLog>.Filter.Gte(w => w.Date, startDate),
                Builders<WorkoutLog>.Filter.Lte(w => w.Date, endDate)
            );
            
            return await _workoutLogsCollection.Find(filter).ToListAsync();
        }
        
        public async Task CreateWorkoutLogAsync(WorkoutLog workoutLog)
        {
            await _workoutLogsCollection.InsertOneAsync(workoutLog);
        }
        
        public async Task UpdateWorkoutLogAsync(string id, WorkoutLog workoutLog)
        {
            await _workoutLogsCollection.ReplaceOneAsync(w => w.Id == id, workoutLog);
        }
        
        public async Task DeleteWorkoutLogAsync(string id)
        {
            await _workoutLogsCollection.DeleteOneAsync(w => w.Id == id);
        }
        
        // User Profile methods
        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            return await _userProfilesCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
        }
        
        public async Task CreateUserProfileAsync(UserProfile userProfile)
        {
            await _userProfilesCollection.InsertOneAsync(userProfile);
        }
        
        public async Task UpdateUserProfileAsync(string userId, UserProfile userProfile)
        {
            await _userProfilesCollection.ReplaceOneAsync(u => u.UserId == userId, userProfile);
        }
        
        public async Task AddWeightEntryAsync(string userId, WeightEntry weightEntry)
        {
            var update = Builders<UserProfile>.Update.Push(u => u.WeightHistory, weightEntry);
            await _userProfilesCollection.UpdateOneAsync(u => u.UserId == userId, update);
        }
    }
}
