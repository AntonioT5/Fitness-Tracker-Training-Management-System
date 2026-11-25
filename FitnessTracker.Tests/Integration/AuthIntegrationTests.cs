using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessTracker.Tests.Integration
{
    [Collection("Api")]
    public class AuthIntegrationTests
    {
        private readonly ApiFactory _factory;

        public AuthIntegrationTests(ApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetGyms_WithoutToken_Returns401()
        {
            var client = _factory.CreateApiClient();

            var response = await client.GetAsync("/api/gym");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithUnknownUser_Returns401()
        {
            var client = _factory.CreateApiClient();

            var response = await client.PostAsJsonAsync("/api/auth/login", new {Email = "nobody@gmail.com", Password = "PW123!"});

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithWorngPassword_Returns401()
        {
            var client = _factory.CreateApiClient();

            var email = $"{Guid.NewGuid()}@test.com";
            var register = await client.PostAsJsonAsync("api/auth/register", new { FirstName = "Test", LastName = "User", Email = email, Password = "Test1234!" });
            Assert.True(register.IsSuccessStatusCode);

            var response = await client.PostAsJsonAsync("api/auth/login", new { Email = email, Password = "Wrong1234!" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var client = _factory.CreateApiClient();
            var email = $"{Guid.NewGuid():N}@test.com";
            var register = await client.PostAsJsonAsync("api/auth/register", new { FirstName = "Test", LastName = "User", Email = email, Password = "Test1234!" });
            Assert.True(register.IsSuccessStatusCode);

            var response = await client.PostAsJsonAsync("api/auth/login", new { Email = email, Password = "Test1234!" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("token").GetString()));
        }

        [Theory]
        [InlineData("Test1234!", HttpStatusCode.OK)]
        [InlineData("Te1!", HttpStatusCode.BadRequest)]
        [InlineData("Testtest!", HttpStatusCode.BadRequest)]
        [InlineData("test1234!", HttpStatusCode.BadRequest)]
        [InlineData("TEST1234!", HttpStatusCode.BadRequest)]
        [InlineData("Test12345", HttpStatusCode.BadRequest)]
        public async Task Register_PasswordRules(string password, HttpStatusCode code)
        {
            var client = _factory.CreateApiClient();
            var email = $"{Guid.NewGuid()}@test.com";

            var response = await client.PostAsJsonAsync("/api/auth/register", new {FirstName = "Test", LastName = "User", Email = email, Password = password});

            Assert.Equal(code, response.StatusCode);
        }
        
    }
}