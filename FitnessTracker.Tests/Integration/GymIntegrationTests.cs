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
    public class GymIntegrationTests
    {
        private readonly ApiFactory _factory;

        public GymIntegrationTests(ApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetGyms_WithValidToken_Returns200()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();

            var response = await client.GetAsync("/api/gym");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateGym_ThenGetById_ReturnsTheSameGym()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var name = $"Gym {Guid.NewGuid()}";

            var create = await client.PostAsJsonAsync("/api/gym", new { Name = name, Address = "Test street 1", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.OK, create.StatusCode);

            var created = await create.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("id").GetGuid();

            var response = await client.GetAsync($"/api/gym/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var fetched = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(name, fetched.GetProperty("name").GetString());
        }
        [Fact]
        public async Task UpdateGym_WithOutGoodId_NotFound()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var name = $"Gym {Guid.NewGuid()}";

            var create = await client.PostAsJsonAsync("/api/gym", new { Name = name, Address = "Test street 1", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.OK, create.StatusCode);

            var id = Guid.NewGuid();

            var response = await client.PutAsJsonAsync($"/api/gym/{id}", new { Name = name, Address = "Test street 12", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGym_WithExistingId_Returns200()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var name = $"Gym {Guid.NewGuid()}";

            var create = await client.PostAsJsonAsync("/api/gym", new { Name = name, Address = "Test street 1", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.OK, create.StatusCode);

            var created = await create.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("id").GetGuid();

            var response = await client.PutAsJsonAsync($"/api/gym/{id}", new { Name = name, Address = "Test street 12", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var get = await client.GetAsync($"/api/gym/{id}");
            var updatedGym = await get.Content.ReadFromJsonAsync<JsonElement>();

            Assert.Equal("Test street 12", updatedGym.GetProperty("address").GetString());
        }

        [Fact]
        public async Task DeleteGym_WithNotGoodId_NotFound()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var name = $"Gym {Guid.NewGuid()}";

            var create = await client.PostAsJsonAsync("/api/gym", new { Name = name, Address = "Test street 1", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.OK, create.StatusCode);

            var id = Guid.NewGuid();

            var response = await client.DeleteAsync($"/api/gym/{id}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGym_WithExistingId_Returns200()
        {
            var client = await _factory.CreateAuthenticatedClientAsync();
            var name = $"Gym {Guid.NewGuid()}";

            var create = await client.PostAsJsonAsync("/api/gym", new { Name = name, Address = "Test street 1", Phone = "070123456", Email = "gym@test.com" });
            Assert.Equal(HttpStatusCode.OK, create.StatusCode);

            var created = await create.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("id").GetGuid();

            var response = await client.DeleteAsync($"/api/gym/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var get = await client.GetAsync($"/api/gym/{id}");
            Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        }

    }
}