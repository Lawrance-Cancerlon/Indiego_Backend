using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface IUserRepository<T> where T : User
{
    Task<List<T>> Get(string? id = null, string? email = null);
    Task<T?> Create(T entity);
    Task<T?> Update(string id, T entity);
    Task<T?> Delete(string id);
}

public class UserRepository<T>(IDatabaseService database) : IUserRepository<T> where T : User
{
    private readonly IMongoCollection<T> _collection = GetCollection(database);

    private static IMongoCollection<T> GetCollection(IDatabaseService database)
    {
        return typeof(T) switch
        {
            Type t when t == typeof(Developer) => database.Developers as IMongoCollection<T> ?? throw new InvalidOperationException("Invalid collection for Developer"),
            Type t when t == typeof(Customer) => database.Customers as IMongoCollection<T> ?? throw new InvalidOperationException("Invalid collection for Customer"),
            Type t when t == typeof(Admin) => database.Admins as IMongoCollection<T> ?? throw new InvalidOperationException("Invalid collection for Admin"),
            Type t when t == typeof(User) => database.Users as IMongoCollection<T> ?? throw new InvalidOperationException("Invalid collection for User"),
            _ => throw new InvalidOperationException("Invalid type for user repository")
        };
    }
    
    public async Task<List<T>> Get(string? id = null, string? email = null)
    {
        var filter = Builders<T>.Filter.Empty;
        filter &= Builders<T>.Filter.AnyEq("_t", typeof(T).Name);
        if (!string.IsNullOrEmpty(id)) filter &= Builders<T>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(email)) filter &= Builders<T>.Filter.Eq(x => x.Email, email);

        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<T?> Create(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<T?> Update(string id, T entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<T?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
