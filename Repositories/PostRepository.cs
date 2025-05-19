using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IPostRepository
{
    Task<List<Post>> Get(string? id = null, string? userId = null);
    Task<List<Post>> GetLikes(List<string> postIds);
    Task<Post?> Create(Post entity);
    Task<Post?> Update(string id, Post entity);
    Task<Post?> Delete(string id);
}

public class PostRepository(IDatabaseService database) : IPostRepository
{
    private readonly IMongoCollection<Post> _collection = database.Posts;

    public async Task<List<Post>> Get(string? id = null, string? userId = null)
    {
        var filter = Builders<Post>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<Post>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(userId)) filter &= Builders<Post>.Filter.Eq(x => x.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<Post>> GetLikes(List<string> postIds)
    {
        var filter = Builders<Post>.Filter.In(x => x.Id, postIds);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Post?> Create(Post entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Post?> Update(string id, Post entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<Post?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
