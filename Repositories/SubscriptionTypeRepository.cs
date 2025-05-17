using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface ISubscriptionTypeRepository
{
    Task<List<SubscriptionType>> Get(string? id = null);
    Task<SubscriptionType?> Create(SubscriptionType entity);
    Task<SubscriptionType?> Update(string id, SubscriptionType entity);
    Task<SubscriptionType?> Delete(string id);
}

public class SubscriptionTypeRepository(IDatabaseService database) : ISubscriptionTypeRepository
{
    private readonly IMongoCollection<SubscriptionType> _collection = database.SubscriptionTypes;

    public async Task<List<SubscriptionType>> Get(string? id = null)
    {
        var filter = Builders<SubscriptionType>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<SubscriptionType>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<SubscriptionType?> Create(SubscriptionType entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<SubscriptionType?> Update(string id, SubscriptionType entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<SubscriptionType?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
