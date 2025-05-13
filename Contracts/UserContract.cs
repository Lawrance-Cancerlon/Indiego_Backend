using System.Text.Json.Serialization;

namespace Indiego_Backend.Contracts;

public record class UserContract
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;
    [JsonPropertyName("role")]
    public string Role { get; init; } = null!;
}

public record class CreateUserContract
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;
    [JsonPropertyName("password")]
    public string Password { get; init; } = null!;
}

public record class UpdateUserContract
{
    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;
    [JsonPropertyName("email")]
    public string? Email { get; init; } = null;
    [JsonPropertyName("password")]
    public string? Password { get; init; } = null;
}

public record class LoginContract
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;
    [JsonPropertyName("password")]
    public string Password { get; init; } = null!;
}

public record class LoginResponseContract
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = null!;
    [JsonPropertyName("user")]
    public UserContract User { get; init; } = null!;
}