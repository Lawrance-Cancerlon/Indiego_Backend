using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Genre
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("GameIds")]
    public List<string> GameIds { get; set; } = [];
}
