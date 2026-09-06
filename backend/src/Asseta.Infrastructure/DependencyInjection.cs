using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.Services;
using Asseta.Infrastructure.Persistence;
using Asseta.Infrastructure.Repositories;
using Asseta.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asseta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration? configuration = null)
    {
        var connectionString = configuration?.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Port=5432;Database=asseta_db;Username=postgres;Password=postgres";

        services.AddDbContext<AssetaDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AssetaDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IAssetaDbContext>(sp => sp.GetRequiredService<AssetaDbContext>());

        services.AddSingleton<IAssetRepository, InMemoryAssetRepository>();
        services.AddScoped<ContinuityMapService>();
        services.AddSingleton<IIdempotencyService, RedisIdempotencyService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPairingCodeHasher, PairingCodeHasher>();

        return services;
    }
}
