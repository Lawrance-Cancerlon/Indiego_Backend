using System;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using MongoDB.Driver;

namespace Indiego_Backend.Repositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> Get(string? id = null, string? userId = null);
    Task<Transaction?> Create(Transaction entity);
    Task<Transaction?> Update(string id, Transaction entity);
    Task<Transaction?> Delete(string id);
}

public class TransactionRepository(IDatabaseService database) : ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _collection = database.Transactions;

    public async Task<List<Transaction>> Get(string? id = null, string? userId = null)
    {
        var filter = Builders<Transaction>.Filter.Empty;
        if (!string.IsNullOrEmpty(id)) filter &= Builders<Transaction>.Filter.Eq(x => x.Id, id);
        if (!string.IsNullOrEmpty(userId)) filter &= Builders<Transaction>.Filter.Eq(x => x.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Transaction?> Create(Transaction entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<Transaction?> Update(string id, Transaction entity)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
    }

    public async Task<Transaction?> Delete(string id)
    {
        var target = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (target == null) return null;
        return await _collection.FindOneAndDeleteAsync(x => x.Id == id);
    }
}
