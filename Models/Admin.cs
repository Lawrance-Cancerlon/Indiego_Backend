using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Indiego_Backend.Models;

public class Admin : User
{
    [BsonElement("CanManageAdmins")]
    public bool CanManageAdmins { get; set; }
    [BsonElement("CanManageUsers")]
    public bool CanManageUsers { get; set; }
    [BsonElement("CanManageGames")]
    public bool CanManageGames { get; set; }
    [BsonElement("CanManagePosts")]
    public bool CanManagePosts { get; set; }
    [BsonElement("CanManageReviews")]
    public bool CanManageReviews { get; set; }
    [BsonElement("CanManageSubscriptions")]
    public bool CanManageSubscriptions { get; set; }
}
