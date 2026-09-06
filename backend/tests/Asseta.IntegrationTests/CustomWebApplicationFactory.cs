using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Constants;
using Asseta.Domain.Entities;
using Asseta.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asseta.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = "IntegrationTestsDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing AssetaDbContext registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AssetaDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AssetaDbContext));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }

            var iContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAssetaDbContext));
            if (iContextDescriptor != null)
            {
                services.Remove(iContextDescriptor);
            }

            // Register InMemory database for integration testing
            services.AddDbContext<AssetaDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddScoped<IAssetaDbContext>(sp => sp.GetRequiredService<AssetaDbContext>());

            // Build service provider and seed database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AssetaDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
