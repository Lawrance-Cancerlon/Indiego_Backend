using System;
using Indiego_Backend.Models.Games;
using Indiego_Backend.Settings;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IGenreRepository
{
    Task<List<Genre>> Get(string? id = null);
    Task<Genre?> Create(Genre genre);
    Task<Genre?> Update(string id, Genre genre);
    Task<Genre?> Delete(string id);
}

public class GenreRepository(DatabaseSetting setting) : IGenreRepository
{
    private readonly IMongoCollection<Genre> _collection = new MongoClient(setting.ConnectionString).GetDatabase(setting.DatabaseName).GetCollection<Genre>("genres");

    public async Task<List<Genre>> Get(string? id = null)
    {
        var filterBuilder = Builders<Genre>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(p => p.Id, id);
        return await _collection.Find(filter).ToListAsync();
    }
    
    public async Task<Genre?> Create(Genre genre)
    {
        await _collection.InsertOneAsync(genre);
        return genre;
    }

    public async Task<Genre?> Update(string id, Genre genre)
    {
        Genre target = await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(p => p.Id == id, genre);
    }

    public async Task<Genre?> Delete(string id)
    {
        Genre target = await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(p => p.Id == id);
    }
}
