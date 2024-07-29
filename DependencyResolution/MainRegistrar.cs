using Application;
using Main;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace DependencyResolution;

public static class BaseRegistrar
{
    public static IServiceCollection AddBaseRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMainRegistration(configuration);

        services.AddPersistenceRegistration(configuration);

        services.AddApplicationRegistration(configuration);

        return services;
    }
}
