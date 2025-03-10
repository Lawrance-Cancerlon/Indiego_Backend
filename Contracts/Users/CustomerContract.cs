using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class CustomerContract : UserContract
{
    [JsonPropertyName("BirthDate")]
    public DateTime BirthDate { get; set; }
    [JsonPropertyName("HasSubscription")]
    public bool HasSubscription { get; set; }
}

public record class CreateCustomerContract : CreateUserContract
{
    [JsonPropertyName("BirthDate")]
    public DateTime BirthDate { get; set; }
}

public record class UpdateCustomerContract : UpdateUserContract
{
    [JsonPropertyName("BirthDate")]
    public DateTime? BirthDate { get; set; }
}
