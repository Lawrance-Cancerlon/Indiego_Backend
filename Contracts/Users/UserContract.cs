using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts.Users;

public record class UserContract
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;
}

public record class CreateUserContract
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

public record class UpdateUserContract
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}

public record class LoginContract
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

public record class TokenContract
{
    [JsonPropertyName("user")]
    public UserContract User { get; set; } = null!;
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
}