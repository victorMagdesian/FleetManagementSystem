using FleetManager.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FleetManager.Tests.Integration;

public class FleetManagerWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; } = $"FleetManagerTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<FleetManagerDbContext>));
            services.RemoveAll(typeof(FleetManagerDbContext));

            // Add DbContext using in-memory database with unique name for each test
            services.AddDbContext<FleetManagerDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });

            // Remove Redis cache service for testing
            services.RemoveAll(typeof(StackExchange.Redis.IConnectionMultiplexer));
            services.RemoveAll(typeof(FleetManager.Application.Interfaces.ICacheService));
        });

        builder.UseEnvironment("Testing");
    }
}
