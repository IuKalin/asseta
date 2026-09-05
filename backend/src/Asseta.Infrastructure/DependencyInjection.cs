using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ContinuityMap.Services;
using Asseta.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Asseta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAssetRepository, InMemoryAssetRepository>();
        services.AddScoped<ContinuityMapService>();
        return services;
    }
}
