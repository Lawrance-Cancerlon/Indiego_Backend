using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class PostContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("title")]
    public string Title { get; init; } = null!;
}
