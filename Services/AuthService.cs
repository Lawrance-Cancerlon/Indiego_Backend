using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Indiego_Backend.Models.Users;
using Indiego_Backend.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Indiego_Backend.Services;

public class AuthService(IDistributedCache cache, JwtSetting setting)
{
    private readonly IDistributedCache _cache = cache;
    private readonly JwtSetting _setting = setting;

    public string GenerateToken(User user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        ];
        if(user is Admin admin)
        {
            claims.Add(new Claim("Role", "Admin"));
            foreach(var prop in admin.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if(prop.PropertyType == typeof(bool) && prop.GetValue(admin) is bool value && value)
                {
                    claims.Add(new Claim(prop.Name, value.ToString()));
                }
            }
        }
        else if(user is Developer developer)
        {
            claims.Add(new Claim("Role", "Developer"));
            if(developer.HasSubscription)
            {
                claims.Add(new Claim("Subscription", "true"));
            }
        }
        else if(user is Customer customer)
        {
            claims.Add(new Claim("Role", "Customer"));
            if(customer.HasSubscription)
            {
                claims.Add(new Claim("Subscription", "true"));
            }
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = _setting.Issuer,
            Audience = _setting.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.Key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task HandleTokenValidation(TokenValidatedContext context)
    {
        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(!string.IsNullOrEmpty(userId))
        {
            var lastActiveKey = $"LastActive_{userId}";
            await _cache.SetStringAsync(lastActiveKey, DateTime.UtcNow.ToString(), new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }
    }

    public async Task HandleTokenReceived(MessageReceivedContext context)
    {
        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(!string.IsNullOrEmpty(userId))
        {
            var lastActiveKey = $"LastActive_{userId}";
            var lastActive = await _cache.GetStringAsync(lastActiveKey);
            if(!string.IsNullOrEmpty(lastActive) && DateTime.UtcNow - DateTime.Parse(lastActive) > TimeSpan.FromDays(1))
            {
                context.Fail("Token is expired");
            }
        }
    }

    public string? GetId(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            return userIdClaim?.Value;
        }
        catch
        {
            return null;
        }
    }

    public void ConfigureJwtOptions(JwtBearerOptions options)
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _setting.Issuer,
            ValidAudience = _setting.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = HandleTokenValidation,
            OnMessageReceived = HandleTokenReceived
        };
    }
}
