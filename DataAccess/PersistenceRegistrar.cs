using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Students;

namespace Persistence;

public static class PersistenceRegistrar
{
    public static IServiceCollection AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStudentRepository, StudentRepository>();

        return services;
    }
}
