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
    public List<string> Downloads { get; init; } = null!;
    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; init; }
    [JsonPropertyName("isSubscribed")]
    public bool IsSubscribed { get; init; } = false;
}

public record class CreateCustomerContract : CreateUserContract
{
    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; init; }
}

public record class UpdateCustomerContract : UpdateUserContract
{
    [JsonPropertyName("birthDate")]
    public DateTime? BirthDate { get; init; } = null;
}