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
        var rawConnectionString = configuration?.GetConnectionString("DefaultConnection") 
            ?? configuration?["DATABASE_URL"]
            ?? Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? "Host=localhost;Port=5432;Database=asseta_db;Username=postgres;Password=postgres";

        var connectionString = ParseConnectionString(rawConnectionString);

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

    /// <summary>
    /// Chuyển đổi định dạng URL PostgreSQL (postgres://user:pass@host:port/db) từ các nền tảng đám mây (Render, Railway, Supabase)
    /// sang chuẩn chuỗi kết nối Npgsql (Host=...;Database=...;) kèm hỗ trợ SSL bắt buộc trên cloud.
    /// </summary>
    private static string ParseConnectionString(string rawConnection)
    {
        if (string.IsNullOrWhiteSpace(rawConnection))
            return "Host=localhost;Port=5432;Database=asseta_db;Username=postgres;Password=postgres";

        if (rawConnection.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            rawConnection.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var uri = new Uri(rawConnection);
                var userInfo = uri.UserInfo.Split(':', 2);
                var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
                var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');

                return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Prefer;Trust Server Certificate=true;";
            }
            catch
            {
                return rawConnection;
            }
        }

        return rawConnection;
    }
}
