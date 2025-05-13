using System;
using Indiego_Backend.Models;
using Indiego_Backend.Settings;
using MongoDB.Driver;

namespace Indiego_Backend.Services;

public interface IDatabaseService
{
    IMongoCollection<User> Users { get; }
    IMongoCollection<Customer> Customers { get; }
    IMongoCollection<Developer> Developers { get; }
    IMongoCollection<Admin> Admins { get; }
}

public class DatabaseService(DatabaseSetting database) : IDatabaseService
{
    private readonly IMongoDatabase _database = new MongoClient(database.ConnectionString).GetDatabase(database.DatabaseName);

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("users");
    public IMongoCollection<Developer> Developers => _database.GetCollection<Developer>("users");
    public IMongoCollection<Admin> Admins => _database.GetCollection<Admin>("users");
}
