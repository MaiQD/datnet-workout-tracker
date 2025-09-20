using MongoDB.Driver;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

public static class UsersMongoIndexConfigurator
{
    public static void Configure(IMongoDatabase database)
    {
        // User indexes
        var userCollection = database.GetCollection<User>("users");
        var userIndexBuilder = Builders<User>.IndexKeys;
        userCollection.Indexes.CreateMany([
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.Email), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.GoogleId)),
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.CreatedAt)),
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.Roles))
        ]);

        // UserMetric indexes
        var userMetricCollection = database.GetCollection<UserMetric>("userMetrics");
        var userMetricIndexBuilder = Builders<UserMetric>.IndexKeys;
        userMetricCollection.Indexes.CreateMany([
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.Date)),
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId).Descending(x => x.Date)),
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId).Ascending(x => x.Date), new CreateIndexOptions { Unique = true })
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
