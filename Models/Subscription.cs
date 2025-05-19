using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Subscription
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("UserId")]
    public string UserId { get; set; } = null!;
    [BsonElement("SubscriptionTypeId")]
    public string SubscriptionTypeId { get; set; } = null!;
    [BsonElement("Download")]
    public int Download { get; set; } = 0;
    [BsonElement("Expire")]
    public string Expire { get; set; } = null!;
}
