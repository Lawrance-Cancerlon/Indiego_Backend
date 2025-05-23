using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class CustomerContract : UserContract
{
    [JsonPropertyName("subscriptionId")]
    public string? SubscriptionId { get; init; } = null!;
    [JsonPropertyName("reviewIds")]
    public List<string> ReviewIds { get; init; } = null!;
    [JsonPropertyName("likes")]
    public List<string> Likes { get; init; } = null!;
    [JsonPropertyName("favorites")]
    public List<string> Favorites { get; init; } = null!;
    [JsonPropertyName("downloads")]
    public List<Download> Downloads { get; init; } = null!;
    [JsonPropertyName("birthDate")]
    public string BirthDate { get; init; } = null!;
    [JsonPropertyName("isSubscribed")]
    public bool IsSubscribed { get; init; }
}

public record class CreateCustomerContract : CreateUserContract
{
    [JsonPropertyName("birthDate")]
    public string BirthDate { get; init; } = null!;
}

public record class UpdateCustomerContract : UpdateUserContract
{
    [JsonPropertyName("birthDate")]
    public string? BirthDate { get; init; } = null;
}