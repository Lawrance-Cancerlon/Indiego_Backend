using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Developer : Customer
{
    [BsonElement("GameIds")]
    public List<string> GameIds { get; set; } = [];
    [BsonElement("PostIds")]
    public List<string> PostIds { get; set; } = [];
    [BsonElement("Balance")]
    public decimal Balance { get; set; } = 0;
}
