using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class ReviewContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("gameId")]
    public string GameId { get; init; } = null!;
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("text")]
    public string Text { get; init; } = null!;
    [JsonPropertyName("rating")]
    public int Rating { get; init; }
    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; init; } = null!;
}

public record class CreateReviewContract
{
    [JsonPropertyName("text")]
    public string Text { get; init; } = null!;
    [JsonPropertyName("rating")]
    public int Rating { get; init; }
}

public record class UpdateReviewContract
{
    [JsonPropertyName("text")]
    public string? Text { get; init; } = null;
    [JsonPropertyName("rating")]
    public int? Rating { get; init; } = null;
}