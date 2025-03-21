using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Games;

public record class GenreContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
}

public record class CreateGenreContract
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
}