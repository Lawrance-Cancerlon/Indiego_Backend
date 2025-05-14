using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("UserId")]
    public string UserId { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("Text")]
    public string Text { get; set; } = null!;
    [BsonElement("Likes")]
    public List<string> Likes { get; set; } = [];
}
