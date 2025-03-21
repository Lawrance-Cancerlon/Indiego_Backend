using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Games;

public record class GameContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    [JsonPropertyName("genreIds")]
    public List<string> GenreIds { get; init; } = null!;
    [JsonPropertyName("downloads")]
    public int Downloads { get; init; } = 0;
    [JsonPropertyName("developerId")]
    public string DeveloperId { get; init; } = null!;
    [JsonPropertyName("rating")]
    public double Rating { get; init; } = 0;
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
    public string? Name { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("genreIds")]
    public List<string>? GenreIds { get; init; }
}