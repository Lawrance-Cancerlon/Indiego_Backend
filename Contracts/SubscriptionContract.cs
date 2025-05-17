using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class SubscriptionContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("subscriptionTypeId")]
    public string SubscriptionTypeId { get; init; } = null!;
    [JsonPropertyName("expire")]
    public string Expire { get; init; } = null!;
}

public record class CreateSubscriptionContract
{
    [JsonPropertyName("subscriptionTypeId")]
    public string SubscriptionTypeId { get; init; } = null!;
}