using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class DeveloperContract : CustomerContract
{
    [JsonPropertyName("gameIds")]
    public List<string> GameIds { get; init; } = null!;
    [JsonPropertyName("postIds")]
    public List<string> PostIds { get; init; } = null!;
    [JsonPropertyName("balance")]
    public int Balance { get; init; }
}