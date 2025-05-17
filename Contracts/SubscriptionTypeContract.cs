using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class SubscriptionTypeContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("price")]
    public int Price { get; init; }
    [JsonPropertyName("duration")]
    public int Duration { get; init; }
}

public record class CreateSubscriptionTypeContract
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("price")]
    public int Price { get; init; }
    [JsonPropertyName("duration")]
    public int Duration { get; init; }
}

public record class UpdateSubscriptionTypeContract
{
    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;
    [JsonPropertyName("description")]
    public string? Description { get; init; } = null;
    [JsonPropertyName("price")]
    public int? Price { get; init; } = null;
    [JsonPropertyName("duration")]
    public int? Duration { get; init; } = null;
}
