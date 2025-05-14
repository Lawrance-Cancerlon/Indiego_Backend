using System.Security.Claims;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using Indiego_Backend.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using System.IdentityModel.Tokens.Jwt;

namespace Indiego_Backend.Tests
{
    public class AuthenticationServiceTest
    {
        private Mock<IDistributedCache> _cacheMock = null!;
        private JwtSetting _jwtSetting = null!;
        private AuthenticationService _authService = null!;

        [SetUp]
        public void Setup()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _jwtSetting = new JwtSetting 
            {
                Key = "mysupersecretkey12345mysupersecretkey12345",
                Issuer = "test-issuer",
                Audience = "test-audience"
            };
            _authService = new AuthenticationService(_cacheMock.Object, _jwtSetting);
        }

        [Test]
        public void GenerateToken_ForDeveloper_ContainsCorrectClaims()
        {
            // Arrange
            var developer = new Developer
            {
                Id = "dev123",
                SubscriptionId = "sub123"
            };

            // Act
            var token = _authService.GenerateToken(developer);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.Multiple(() =>
            {
                Assert.That(jwtToken.Claims.First(c => c.Type == "nameid").Value, Is.EqualTo("dev123"));
                Assert.That(jwtToken.Claims.First(c => c.Type == "role").Value, Is.EqualTo("Developer"));
                Assert.That(jwtToken.Claims.First(c => c.Type == "subscription").Value, Is.EqualTo("true"));
            });
        }

        [Test]
        public void GenerateToken_ForCustomer_ContainsCorrectClaims()
        {
            // Arrange
            var customer = new Customer
            {
                Id = "cust123",
                SubscriptionId = null
            };

            // Act
            var token = _authService.GenerateToken(customer);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.Multiple(() =>
            {
                Assert.That(jwtToken.Claims.First(c => c.Type == "nameid").Value, Is.EqualTo("cust123"));
                Assert.That(jwtToken.Claims.First(c => c.Type == "role").Value, Is.EqualTo("Customer"));
                Assert.That(jwtToken.Claims.First(c => c.Type == "subscription").Value, Is.EqualTo("false"));
            });
        }

        [Test]
        public void GetId_ValidToken_ReturnsUserId()
        {
            // Arrange
            var developer = new Developer { Id = "dev123" };
            var token = _authService.GenerateToken(developer);

            // Act
            var userId = _authService.GetId(token);

            // Assert
            Assert.That(userId, Is.EqualTo("dev123"));
        }

        [Test]
        public void GetRole_ValidToken_ReturnsRole()
        {
            // Arrange
            var developer = new Developer { Id = "dev123" };
            var token = _authService.GenerateToken(developer);

            // Act
            var role = _authService.GetRole(token);

            // Assert
            Assert.That(role, Is.EqualTo("Developer"));
        }

        [Test]
        public void ConfigureJwtOptions_SetsCorrectParameters()
        {
            // Arrange
            var options = new JwtBearerOptions();

            // Act
            _authService.ConfigureJwtOptions(options);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(options.RequireHttpsMetadata, Is.False);
                Assert.That(options.SaveToken, Is.True);
                Assert.That(options.TokenValidationParameters.ValidIssuer, Is.EqualTo(_jwtSetting.Issuer));
                Assert.That(options.TokenValidationParameters.ValidAudience, Is.EqualTo(_jwtSetting.Audience));
                Assert.That(options.TokenValidationParameters.ValidateLifetime, Is.True);
                Assert.That(options.Events, Is.Not.Null);
            });
        }
    }
}
