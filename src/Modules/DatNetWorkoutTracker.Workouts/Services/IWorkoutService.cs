using DatNetWorkoutTracker.Workouts.Domain;

namespace DatNetWorkoutTracker.Workouts.Services;

public interface IWorkoutService
{
    Task<Workout?> GetWorkoutByIdAsync(string workoutId);
    Task<IEnumerable<Workout>> GetWorkoutsByUserAsync(string userId);
    Task<IEnumerable<Workout>> GetWorkoutsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Workout>> GetWorkoutsByStatusAsync(string userId, WorkoutStatus status);
    Task<Workout?> GetActiveWorkoutAsync(string userId);
    Task<Workout> CreateWorkoutAsync(Workout workout);
    Task<Workout> UpdateWorkoutAsync(Workout workout);
    Task<Workout> StartWorkoutAsync(string workoutId);
    Task<Workout> CompleteWorkoutAsync(string workoutId);
    Task<Workout> CancelWorkoutAsync(string workoutId);
    Task DeleteWorkoutAsync(string workoutId);
    Task<WorkoutStatistics> GetWorkoutStatisticsAsync(string userId, DateTime? fromDate = null);
}

public class WorkoutStatistics
{
    public int TotalWorkouts { get; set; }
    public TimeSpan TotalWorkoutTime { get; set; }
    public int TotalSets { get; set; }
    public int TotalReps { get; set; }
    public decimal TotalWeightLifted { get; set; }
    public Dictionary<string, int> ExerciseFrequency { get; set; } = new();
    public Dictionary<DayOfWeek, int> WorkoutsByDayOfWeek { get; set; } = new();
}
