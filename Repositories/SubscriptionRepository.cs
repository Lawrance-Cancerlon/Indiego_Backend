using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> Get(string? id = null, string? userId = null);
    Task<Subscription?> Create(Subscription entity);
    Task<Subscription?> Update(string id, Subscription entity);
    Task<Subscription?> Delete(string id);
}

public class SubscriptionRepository(IDatabaseService database) : ISubscriptionRepository
{
    private readonly IMongoCollection<Subscription> _collection = database.Subscriptions;

    public async Task<List<Subscription>> Get(string? id = null, string? userId = null)
    {
        var filter = Builders<Subscription>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<Subscription>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(userId)) filter &= Builders<Subscription>.Filter.Eq(x => x.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Subscription?> Create(Subscription entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Subscription?> Update(string id, Subscription entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<Subscription?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
