using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Transaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("UserId")]
    public string UserId { get; set; } = null!;
    [BsonElement("Amount")]
    public int Amount { get; set; }
    [BsonElement("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
