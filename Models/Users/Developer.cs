using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models.Users;

public class Developer : Customer
{
    [BsonElement("Balance")]
    public decimal Balance { get; set; } = 0;
    [BsonElement("Bank")]
    public string Bank { get; set; } = null!;
    [BsonElement("AccountNumber")]
    public string AccountNumber { get; set; } = null!;
    [BsonElement("AccountName")]
    public string AccountName { get; set; } = null!;
}
