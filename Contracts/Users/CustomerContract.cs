using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class CustomerContract : UserContract
{
    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; set; }
    [JsonPropertyName("hasSubscription")]
    public bool HasSubscription { get; set; }
    [JsonPropertyName("favorites")]
    public List<string> Favorites { get; set; } = null!;
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
