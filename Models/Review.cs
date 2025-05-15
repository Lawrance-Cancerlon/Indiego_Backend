using System;
using Indiego_Backend.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("GameId")]
    public string GameId { get; set; } = null!;
    [BsonElement("UserId")]
    public string UserId { get; set; } = null!;
    [BsonElement("Text")]
    public string Text { get; set; } = "";
    [BsonElement("Rating")]
    public int Rating { get; set; } = 0;
    [BsonElement("CreatedAt")]
    public string CreatedAt { get; set; } = DatetimeUtility.ToUnixTimestampString(DateTime.Now);
}
