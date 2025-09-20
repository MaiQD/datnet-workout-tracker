using MongoDB.Driver;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

public static class ExercisesMongoIndexConfigurator
{
    public static void Configure(IMongoDatabase database)
    {
        // Exercise indexes
        var exerciseCollection = database.GetCollection<Exercise>("exercises");
        var exerciseIndexBuilder = Builders<Exercise>.IndexKeys;
        exerciseCollection.Indexes.CreateMany([
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.MuscleGroups)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Equipment)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Difficulty)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Tags)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.CreatedAt)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Text(x => x.Name).Text(x => x.Description)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.UserId).Ascending(x => x.IsGlobal).Ascending(x => x.Name), new CreateIndexOptions { Unique = true })
        ]);

        // MuscleGroup indexes
        var muscleGroupCollection = database.GetCollection<MuscleGroup>("muscleGroups");
        var muscleGroupIndexBuilder = Builders<MuscleGroup>.IndexKeys;
        muscleGroupCollection.Indexes.CreateMany([
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.BodyRegion)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.ParentId)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.CreatedAt))
        ]);

        // Equipment indexes
        var equipmentCollection = database.GetCollection<Equipment>("equipment");
        var equipmentIndexBuilder = Builders<Equipment>.IndexKeys;
        equipmentCollection.Indexes.CreateMany([
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.Category)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.CreatedAt))
        ]);

        // UserPreferencesProjection indexes
        var prefs = database.GetCollection<UserPreferencesProjection>("userPreferencesProjections");
        var prefsIndex = Builders<UserPreferencesProjection>.IndexKeys;
        prefs.Indexes.CreateMany([
            new CreateIndexModel<UserPreferencesProjection>(prefsIndex.Ascending(x => x.UserId), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<UserPreferencesProjection>(prefsIndex.Ascending(x => x.UpdatedAt))
        ]);

        // Inbox indexes
        var inboxCollection = database.GetCollection<InboxMessage>("inboxMessages");
        var inboxIndexBuilder = Builders<InboxMessage>.IndexKeys;
        inboxCollection.Indexes.CreateMany([
            new CreateIndexModel<InboxMessage>(inboxIndexBuilder.Ascending(x => x.Consumer).Ascending(x => x.EventId), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<InboxMessage>(inboxIndexBuilder.Ascending(x => x.Consumer).Ascending(x => x.Status).Ascending(x => x.OccurredOn))
        ]);
    }
}
