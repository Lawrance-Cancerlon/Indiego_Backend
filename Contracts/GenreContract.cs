using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class GenreContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("gameIds")]
    public List<string> GameIds { get; init; } = null!;
}

public record class CreateGenreContract
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
}