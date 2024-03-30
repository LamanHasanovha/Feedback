using Application;
using Main.Persistence;
using Main.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace DependencyResolution;

public static class MainRegistrar
{
    public static IServiceCollection AddMainRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSecurityRegistration(configuration);

        services.AddPersistenceCoreRegistration(configuration);

        services.AddPersistenceRegistration(configuration);

        services.AddApplicationRegistration(configuration);

        return services;
    }
}
