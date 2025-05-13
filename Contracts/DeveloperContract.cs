using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class DeveloperContract : CustomerContract
{
    [JsonPropertyName("gameIds")]
    public List<string> GameIds { get; init; } = null!;
    [JsonPropertyName("postIds")]
    public List<string> PostIds { get; init; } = null!;
    [JsonPropertyName("balance")]
    public decimal Balance { get; init; } = 0;
}

public record class UpdateDeveloperContract : UpdateCustomerContract
{
    [JsonPropertyName("gameIds")]
    public List<string>? GameIds { get; init; } = null;
    [JsonPropertyName("postIds")]
    public List<string>? PostIds { get; init; } = null;
}