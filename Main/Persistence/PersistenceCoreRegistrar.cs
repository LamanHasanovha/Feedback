using Microsoft.EntityFrameworkCore;
using Main.Persistence.PersistenceBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Main.Persistence.Authorization;

namespace Main.Persistence;

public static class PersistenceCoreRegistrar
{
    public static IServiceCollection AddPersistenceCoreRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IDbContextProvider<>), typeof(SimpleDbContextProvider<>));

        services.AddDbContext<FeedbackCoreContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();

        return services;
    }
}
