using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class DeveloperContract : CustomerContract
{
    [JsonPropertyName("Bank")]
    public string Bank { get; set; } = null!;
    [JsonPropertyName("AccountNumber")]
    public string AccountNumber { get; set; } = null!;
    [JsonPropertyName("AccountName")]
    public string AccountName { get; set; } = null!;
}

public record CreateDeveloperContract
{
    [JsonPropertyName("Bank")]
    public string Bank { get; set; } = null!;
    [JsonPropertyName("AccountNumber")]
    public string AccountNumber { get; set; } = null!;
    [JsonPropertyName("AccountName")]
    public string AccountName { get; set; } = null!;
}

public record UpdateDeveloperContract : UpdateCustomerContract
{
    [JsonPropertyName("Bank")]
    public string? Bank { get; set; }
    [JsonPropertyName("AccountNumber")]
    public string? AccountNumber { get; set; }
    [JsonPropertyName("AccountName")]
    public string? AccountName { get; set; }
}
