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
    [JsonPropertyName("text")]
    public string Text { get; init; } = null!;
    [JsonPropertyName("likes")]
    public int Likes { get; init; }
    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; init; } = null!;
}

public record class CreatePostContract
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = null!;
    [JsonPropertyName("text")]
    public string Text { get; init; } = null!;
}

public record class UpdatePostContract
{
    [JsonPropertyName("title")]
    public string? Title { get; init; } = null;
    [JsonPropertyName("text")]
    public string? Text { get; init; } = null;
}