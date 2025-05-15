using System;
using Indiego_Backend.Contracts;
using Indiego_Backend.Utilities;
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
    public List<Download> Downloads { get; set; } = [];
    [BsonElement("ReviewIds")]
    public List<string> ReviewIds { get; set; } = [];
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("GenreIds")]
    public List<string> GenreIds { get; set; } = null!;
    [BsonElement("CreatedAt")]
    public string CreatedAt { get; set; } = DatetimeUtility.ToUnixTimestampString(DateTime.Now);
}
