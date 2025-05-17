using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IGameRepository
{
    Task<List<Game>> Get(string? id = null, string? userId = null, string? genreId = null);
    Task<Game?> Create(Game entity);
    Task<Game?> Update(string id, Game entity);
    Task<Game?> Delete(string id);
}

public class GameRepository(IDatabaseService database) : IGameRepository
{
    private readonly IMongoCollection<Game> _collection = database.Games;

    public async Task<List<Game>> Get(string? id = null, string? userId = null, string? genreId = null)
    {
        var filter = Builders<Game>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<Game>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(userId)) filter &= Builders<Game>.Filter.Eq(x => x.UserId, userId);
        if (!string.IsNullOrEmpty(genreId)) filter &= Builders<Game>.Filter.AnyEq(x => x.GenreIds, genreId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Game?> Create(Game entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Game?> Update(string id, Game entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<Game?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
