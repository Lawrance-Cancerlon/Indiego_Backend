using System;
using Indiego_Backend.Contracts;
using Indiego_Backend.Utilities;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Customer : User
{
    [BsonElement("SubscriptionId")]
    public string? SubscriptionId { get; set; } = null;
    [BsonElement("ReviewIds")]
    public List<string> ReviewIds { get; set; } = [];
    [BsonElement("Likes")]
    public List<string> Likes { get; set; } = [];
    [BsonElement("Favorites")]
    public List<string> Favorites { get; set; } = [];
    [BsonElement("Downloads")]
    public List<Download> Downloads { get; set; } = [];
    [BsonElement("BirthDate")]
    public string BirthDate { get; set; } = null!;
}
