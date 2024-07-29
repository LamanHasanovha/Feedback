using Main.Application.Mapper;
using Main.Base.Instance;
using Main.Base.Pluralization;
using Main.Persistence.Authorization;
using Main.Security.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Main;

public static class MainRegistrar
{
    public static IServiceCollection AddMainRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenHelper, TokenHelper>();

        services.AddScoped<IMapper, FeedbackMapper>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        services.AddSingleton<ServiceFactory>();

        services.AddTransient<IPluralizationService, PluralizationService>();

        return services;
    }
}
