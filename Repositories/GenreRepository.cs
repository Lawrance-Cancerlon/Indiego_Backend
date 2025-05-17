using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IGenreRepository
{
    Task<List<Genre>> Get(string? id = null, string? gameId = null);
    Task<Genre?> Create(Genre entity);
    Task<Genre?> Update(string id, Genre entity);
    Task<Genre?> Delete(string id);
}

public class GenreRepository(IDatabaseService database) : IGenreRepository
{
    private readonly IMongoCollection<Genre> _collection = database.Genres;

    public async Task<List<Genre>> Get(string? id = null, string? gameId = null)
    {
        var filter = Builders<Genre>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<Genre>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(gameId)) filter &= Builders<Genre>.Filter.AnyEq(x => x.GameIds, gameId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Genre?> Create(Genre entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Genre?> Update(string id, Genre entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<Genre?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
