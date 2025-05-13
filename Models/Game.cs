using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Game
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("UserId")]
    public string UserId { get; set; } = null!;
    [BsonElement("Downloads")]
    public List<string> Downloads { get; set; } = [];
    [BsonElement("ReviewIds")]
    public List<string> ReviewIds { get; set; } = [];
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("GenreIds")]
    public List<string> GenreIds { get; set; } = null!;
    [BsonElement("Path")]
    public string Path { get; set; } = null!;
}
