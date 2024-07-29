using Application.Authorization;
using Application.Students;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationRegistrar
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStudentService, StudentService>();

        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}
