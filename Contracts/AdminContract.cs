using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Indiego_Backend.Contracts;

public record class AdminContract : UserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool CanManageAdmins { get; init; }
    [JsonPropertyName("canManageUsers")]
    public bool CanManageUsers { get; init; }
    [JsonPropertyName("canManageGames")]
    public bool CanManageGames { get; init; }
    [JsonPropertyName("canManagePosts")]
    public bool CanManagePosts { get; init; }
    [JsonPropertyName("canManageReviews")]
    public bool CanManageReviews { get; init; }
    [JsonPropertyName("canManageTransactions")]
    public bool CanManageTransactions { get; init; }
}

public record class CreateAdminContract : CreateUserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool CanManageAdmins { get; init; }
    [JsonPropertyName("canManageUsers")]
    public bool CanManageUsers { get; init; }
    [JsonPropertyName("canManageGames")]
    public bool CanManageGames { get; init; }
    [JsonPropertyName("canManagePosts")]
    public bool CanManagePosts { get; init; }
    [JsonPropertyName("canManageReviews")]
    public bool CanManageReviews { get; init; }
    [JsonPropertyName("canManageTransactions")]
    public bool CanManageTransactions { get; init; }
}

public record class UpdateAdminContract : UpdateUserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool? CanManageAdmins { get; init; } = null;
    [JsonPropertyName("canManageUsers")]
    public bool? CanManageUsers { get; init; } = null;
    [JsonPropertyName("canManageGames")]
    public bool? CanManageGames { get; init; } = null;
    [JsonPropertyName("canManagePosts")]
    public bool? CanManagePosts { get; init; } = null;
    [JsonPropertyName("canManageReviews")]
    public bool? CanManageReviews { get; init; } = null;
    [JsonPropertyName("canManageTransactions")]
    public bool? CanManageTransactions { get; init; } = null;
}