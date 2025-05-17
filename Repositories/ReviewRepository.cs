using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> Get(string? id = null, string? userId = null, string? gameId = null);
    Task<Review?> Create(Review entity);
    Task<Review?> Update(string id, Review entity);
    Task<Review?> Delete(string id);
}

public class ReviewRepository(IDatabaseService database) : IReviewRepository
{
    private readonly IMongoCollection<Review> _collection = database.Reviews;

    public async Task<List<Review>> Get(string? id = null, string? userId = null, string? gameId = null)
    {
        var filter = Builders<Review>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<Review>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(userId)) filter &= Builders<Review>.Filter.Eq(x => x.UserId, userId);
        if (!string.IsNullOrEmpty(gameId)) filter &= Builders<Review>.Filter.Eq(x => x.GameId, gameId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Review?> Create(Review entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Review?> Update(string id, Review entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<Review?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
