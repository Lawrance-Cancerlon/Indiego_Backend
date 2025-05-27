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
    [JsonPropertyName("devName")]
    public string DevName { get; init; } = null!;
    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = null!;
    [JsonPropertyName("taxId")]
    public string TaxId { get; init; } = null!;
    [JsonPropertyName("country")]
    public string Country { get; init; } = null!;
}

public record class CreateDeveloperContract
{
    [JsonPropertyName("devName")]
    public string DevName { get; init; } = null!;
    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = null!;
    [JsonPropertyName("taxId")]
    public string TaxId { get; init; } = null!;
    [JsonPropertyName("country")]
    public string Country { get; init; } = null!;
}

public record class UpdateDeveloperContract : UpdateCustomerContract
{
    [JsonPropertyName("devName")]
    public string? DevName { get; init; }
    [JsonPropertyName("fullName")]
    public string? FullName { get; init; }
    [JsonPropertyName("taxId")]
    public string? TaxId { get; init; }
    [JsonPropertyName("country")]
    public string? Country { get; init; }
}