using Main.Security.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Main.Security;

public static class SecurityRegistrar
{
    public static IServiceCollection AddSecurityRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenHelper, TokenHelper>();

        return services;
    }
}
