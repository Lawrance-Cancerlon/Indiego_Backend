using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class DeveloperContract : CustomerContract
{
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; } = 0;
    [JsonPropertyName("bank")]
    public string Bank { get; set; } = null!;
    [JsonPropertyName("accountNumber")]
    public string AccountNumber { get; set; } = null!;
    [JsonPropertyName("accountName")]
    public string AccountName { get; set; } = null!;
}

public record CreateDeveloperContract
{
    [JsonPropertyName("bank")]
    public string Bank { get; set; } = null!;
    [JsonPropertyName("accountNumber")]
    public string AccountNumber { get; set; } = null!;
    [JsonPropertyName("accountName")]
    public string AccountName { get; set; } = null!;
}

public record UpdateDeveloperContract : UpdateCustomerContract
{
    [JsonPropertyName("bank")]
    public string? Bank { get; set; }
    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }
    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }
}
