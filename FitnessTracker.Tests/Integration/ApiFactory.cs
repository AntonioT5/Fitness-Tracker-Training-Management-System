using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions.SqlAdapters.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using Service.Jobs;
using Testcontainers.PostgreSql;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FitnessTracker.Tests.Integration
{
    public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime //Testcontainer with WebApplicationFactory
    { 

        private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:16-alpine").Build();
        public async Task InitializeAsync()
        {
            await _db.StartAsync();

            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _db.GetConnectionString());
            Environment.SetEnvironmentVariable("Jwt__Key", "test-secret-key-test-secret-key-1234567890");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "TestIssuer");
            Environment.SetEnvironmentVariable("Jwt__Audience", "TestAudience");

            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Database.GetConnectionString() != _db.GetConnectionString())
                throw new InvalidOperationException("Tests are not using the Testcontainers database!");

            await context.Database.EnsureCreatedAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // iskluchvanje na background jobs
            builder.ConfigureTestServices(services =>
            {
                var backgroundJobs = services
                    .Where(d => d.ServiceType == typeof(IHostedService)
                        && (d.ImplementationType == typeof(EtlBackgroundService)
                        || d.ImplementationType?.Name == "QuartzHostedService")).ToList();
                
                foreach (var job in backgroundJobs)
                    services.Remove(job);
            });
        }

        public HttpClient CreateApiClient() => CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        public async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var client = CreateApiClient();
            var email = $"{Guid.NewGuid():N}@test.com"; 
            const string password = "Test1234!";

            var register = await client.PostAsJsonAsync("/api/auth/register",
                new { FirstName = "Test", LastName = "User", Email = email, Password = password });
            register.EnsureSuccessStatusCode();

            var login = await client.PostAsJsonAsync("/api/auth/login",
                new { Email = email, Password = password });
            login.EnsureSuccessStatusCode();

            var body = await login.Content.ReadFromJsonAsync<JsonElement>();
            var token = body.GetProperty("token").GetString();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _db.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}