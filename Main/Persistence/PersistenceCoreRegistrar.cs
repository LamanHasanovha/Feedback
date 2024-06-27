using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Main.Persistence;

public static class PersistenceCoreRegistrar
{
    public static IServiceCollection AddPersistenceCoreRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
