using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class CustomerContract : UserContract
{
    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; set; }
    [JsonPropertyName("hasSubscription")]
    public bool HasSubscription { get; set; }
}

public record class CreateCustomerContract : CreateUserContract
{
    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; set; }
}

public record class UpdateCustomerContract : UpdateUserContract
{
    [JsonPropertyName("birthDate")]
    public DateTime? BirthDate { get; set; }
}
