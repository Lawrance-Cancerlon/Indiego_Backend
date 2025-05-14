using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class GameContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("userId")]
    public string UserId { get; init; } = null!;
    [JsonPropertyName("downloads")]
    public int Downloads { get; init; } = 0;
    [JsonPropertyName("reviewIds")]
    public List<string> ReviewIds { get; init; } = [];
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("genreIds")]
    public List<string> GenreIds { get; init; } = null!;
}

public record class CreateGameContract
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("genreIds")]
    public List<string> GenreIds { get; init; } = null!;
}

public record class UpdateGameContract
{
    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;
    [JsonPropertyName("description")]
    public string? Description { get; init; } = null;
    [JsonPropertyName("genreIds")]
    public List<string>? GenreIds { get; init; } = null;
}