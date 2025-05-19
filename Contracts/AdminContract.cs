using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Indiego_Backend.Contracts;

public record class AdminContract : UserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool CanManageAdmins { get; init; }
    [JsonPropertyName("canManageGames")]
    public bool CanManageGames { get; init; }
    [JsonPropertyName("canManagePosts")]
    public bool CanManagePosts { get; init; }
    [JsonPropertyName("canManageReviews")]
    public bool CanManageReviews { get; init; }
    [JsonPropertyName("canManageSubscriptions")]
    public bool CanManageSubscriptions { get; init; }
}

public record class CreateAdminContract : CreateUserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool CanManageAdmins { get; init; }
    [JsonPropertyName("canManageGames")]
    public bool CanManageGames { get; init; }
    [JsonPropertyName("canManagePosts")]
    public bool CanManagePosts { get; init; }
    [JsonPropertyName("canManageReviews")]
    public bool CanManageReviews { get; init; }
    [JsonPropertyName("canManageSubscriptions")]
    public bool CanManageSubscriptions { get; init; }
}

public record class UpdateAdminContract : UpdateUserContract
{
    [JsonPropertyName("canManageAdmins")]
    public bool? CanManageAdmins { get; init; } = null;
    [JsonPropertyName("canManageGames")]
    public bool? CanManageGames { get; init; } = null;
    [JsonPropertyName("canManagePosts")]
    public bool? CanManagePosts { get; init; } = null;
    [JsonPropertyName("canManageReviews")]
    public bool? CanManageReviews { get; init; } = null;
    [JsonPropertyName("canManageSubscriptions")]
    public bool? CanManageSubscriptions { get; init; } = null;
}