using System.Text.Json.Serialization;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Contracts;

public record class GameContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("downloads")]
    public int Downloads { get; init; }
    [JsonPropertyName("reviewIds")]
    public List<string> ReviewIds { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("genreIds")]
    public List<string> GenreIds { get; init; } = null!;
    [JsonPropertyName("link")]
    public string? Link { get; init; } = null;
    [JsonPropertyName("rating")]
    public decimal Rating { get; init; }
    [JsonPropertyName("ratingCount")]
    public int RatingCount { get; init; }
    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; init; } = null!;
}

public record class CreateGameContract
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("genreIds")]
    public List<string> GenreIds { get; init; } = null!;
    [JsonPropertyName("link")]
    public string? Link { get; init; } = null;
}

public record class UpdateGameContract
{
    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;
    [JsonPropertyName("description")]
    public string? Description { get; init; } = null;
    [JsonPropertyName("genreIds")]
    public List<string>? GenreIds { get; init; } = null;
    [JsonPropertyName("link")]
    public string? Link { get; init; } = null;
}

public record class Download
{
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("gameId")]
    public string GameId { get; init; } = null!;
    [JsonPropertyName("timestamp")]
    public string Timestamp = DatetimeUtility.ToUnixTimestampString(DateTime.Now);
}