using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models.Users;

public class Customer : User
{
    [BsonElement("BirthDate")]
    public DateTime BirthDate { get; set; }
    [BsonElement("HasSubscription")]
    public bool HasSubscription { get; set; }
    [BsonElement("Favorites")]
    public List<string> Favorites { get; set; } = null!;
}
