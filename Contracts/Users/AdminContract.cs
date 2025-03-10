using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class AdminContract : UserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool CanManageAdmins { get; set; }
    [JsonPropertyName("canManageUsers")]
    public bool CanManageUsers { get; set; }
    [JsonPropertyName("canManageGames")]
    public bool CanManageGames { get; set; }
    [JsonPropertyName("canManagePosts")]
    public bool CanManagePosts { get; set; }
    [JsonPropertyName("canManageReviews")]
    public bool CanManageReviews { get; set; }
    [JsonPropertyName("canManageSubscriptions")]
    public bool CanManageSubscriptions { get; set; }
    [JsonPropertyName("canManageTransactions")]
    public bool CanManageTransactions { get; set; }
}

public record class CreateAdminContract : CreateUserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool CanManageAdmins { get; set; }
    [JsonPropertyName("canManageUsers")]
    public bool CanManageUsers { get; set; }
    [JsonPropertyName("canManageGames")]
    public bool CanManageGames { get; set; }
    [JsonPropertyName("canManagePosts")]
    public bool CanManagePosts { get; set; }
    [JsonPropertyName("canManageReviews")]
    public bool CanManageReviews { get; set; }
    [JsonPropertyName("canManageSubscriptions")]
    public bool CanManageSubscriptions { get; set; }
    [JsonPropertyName("canManageTransactions")]
    public bool CanManageTransactions { get; set; }
}

public record class UpdateAdminContract : UpdateUserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool? CanManageAdmins { get; set; }
    [JsonPropertyName("canManageUsers")]
    public bool? CanManageUsers { get; set; }
    [JsonPropertyName("canManageGames")]
    public bool? CanManageGames { get; set; }
    [JsonPropertyName("canManagePosts")]
    public bool? CanManagePosts { get; set; }
    [JsonPropertyName("canManageReviews")]
    public bool? CanManageReviews { get; set; }
    [JsonPropertyName("canManageSubscriptions")]
    public bool? CanManageSubscriptions { get; set; }
    [JsonPropertyName("canManageTransactions")]
    public bool? CanManageTransactions { get; set; }
}
