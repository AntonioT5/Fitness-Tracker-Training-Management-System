using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;

namespace FitnessTracker.Tests.Integration
{
    [Collection("Api")]
    public class ApiKeyIntegrationTests
    {
        private readonly ApiFactory _factory;

        public ApiKeyIntegrationTests(ApiFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ReceiveWorkoutSession_WithoutApiKey_Returns401()
        {
            var client = _factory.CreateApiClient();

            var request = new InboundWorkoutSessionRequest(
                "member@test.com",
                "Test Trainer",
                "Bench Press",
                DateTime.UtcNow,
                3,
                10,
                50,
                30,
                "Integration test"
            );

            var response = await client.PostAsJsonAsync("/api/external/workoutSessions", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ReceiveWorkoutSession_WithInvalidApiKey_Returns401()
        {
            var client = _factory.CreateApiClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", "invalid-api-key");

            var request = new InboundWorkoutSessionRequest(
                "member@test.com",
                "Test Trainer",
                "Bench Press",
                DateTime.UtcNow,
                3,
                10,
                50,
                30,
                "Integration test"
            );

            var response = await client.PostAsJsonAsync("/api/external/workoutSessions", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ReceiveWorkoutSession_WithValidApiKey_Returns202()
        {
            var apiKey = $"test-{Guid.NewGuid()}";

            var apiClient = new ApiClient
            {
                Name = "Integration Test Client",
                ApiKey = apiKey,
                IsActive = true,
                RateLimitMinutes = 10
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.ApiClients.Add(apiClient);
                await dbContext.SaveChangesAsync();
            }

            var client = _factory.CreateApiClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            var request = new InboundWorkoutSessionRequest(
                "member@test.com",
                "Test Trainer",
                "Bench Press",
                DateTime.UtcNow,
                3,
                10,
                50,
                30,
                "Integration test"
            );

            var response = await client.PostAsJsonAsync("/api/external/workoutSessions",request);
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("pending", body.GetProperty("status").GetString());
        }

        [Fact]
        public async Task ReceiveWorkoutSession_WithInValidApiKey_Returns401()
        {
            var apiKey = $"test-{Guid.NewGuid()}";

            var apiClient = new ApiClient
            {
                Name = "Integration Test Client",
                ApiKey = apiKey,
                IsActive = false,
                RateLimitMinutes = 10
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.ApiClients.Add(apiClient);
                await dbContext.SaveChangesAsync();
            }

            var client = _factory.CreateApiClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            var request = new InboundWorkoutSessionRequest(
                "member@test.com",
                "Test Trainer",
                "Bench Press",
                DateTime.UtcNow,
                3,
                10,
                50,
                30,
                "Integration test"
            );

            var response = await client.PostAsJsonAsync("/api/external/workoutSessions",request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}