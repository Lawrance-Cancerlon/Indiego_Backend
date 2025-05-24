using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Developer : Customer
{
    [BsonElement("DevName")]
    public string DevName { get; set; } = null!;
    [BsonElement("FullName")]
    public string FullName { get; set; } = null!;
    [BsonElement("TaxId")]
    public string TaxId { get; set; } = null!;
    [BsonElement("Country")]
    public string Country { get; set; } = null!;
    [BsonElement("GameIds")]
    public List<string> GameIds { get; set; } = [];
    [BsonElement("PostIds")]
    public List<string> PostIds { get; set; } = [];
    [BsonElement("Balance")]
    public int Balance { get; set; } = 0;
}
