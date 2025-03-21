using System;
using Indiego_Backend.Models.Games;
using Indiego_Backend.Settings;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IGameRepository
{
    Task<List<Game>> Get(string? id = null);
    Task<Game?> Create(Game game);
    Task<Game?> Update(string id, Game game);
    Task<Game?> Delete(string id);
}

public class GameRepository(DatabaseSetting setting) : IGameRepository
{
    private readonly IMongoCollection<Game> _collection = new MongoClient(setting.ConnectionString).GetDatabase(setting.DatabaseName).GetCollection<Game>("games");

    public async Task<List<Game>> Get(string? id = null)
    {
        var filterBuilder = Builders<Game>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(p => p.Id, id);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Game?> Create(Game game)
    {
        await _collection.InsertOneAsync(game);
        return game;
    }

    public async Task<Game?> Update(string id, Game game)
    {
        Game target = await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(p => p.Id == id, game);
    }

    public async Task<Game?> Delete(string id)
    {
        Game target = await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(p => p.Id == id);
    }
}
