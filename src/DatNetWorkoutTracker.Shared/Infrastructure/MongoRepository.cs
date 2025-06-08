using MongoDB.Driver;
using System.Linq.Expressions;
using DatNetWorkoutTracker.Shared.Domain;

namespace DatNetWorkoutTracker.Shared.Infrastructure;

public class MongoRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(x => !x.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(Builders<T>.Filter.And(
            Builders<T>.Filter.Where(predicate),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false)
        )).ToListAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        return entity;
    }

    public async Task DeleteAsync(string id)
    {
        var update = Builders<T>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);
        
        await _collection.UpdateOneAsync(x => x.Id == id, update);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _collection.Find(x => x.Id == id && !x.IsDeleted).AnyAsync();
    }
}
