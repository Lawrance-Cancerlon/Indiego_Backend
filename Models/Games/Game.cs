using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models.Games;

public class Game
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("GenreIds")]
    public List<string> GenreIds { get; set; } = null!;
    [BsonElement("Downloads")]
    public int Downloads { get; set; } = 0;
    [BsonElement("DeveloperId")]
    public string DeveloperId { get; set; } = null!;
}