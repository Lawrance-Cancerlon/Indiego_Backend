using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models.Users;

public class Developer : Customer
{
    [BsonElement("Bank")]
    public string Bank { get; set; } = null!;
    [BsonElement("AccountNumber")]
    public string AccountNumber { get; set; } = null!;
    [BsonElement("AccountName")]
    public string AccountName { get; set; } = null!;
}
