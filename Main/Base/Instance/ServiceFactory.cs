using Main.Base.Pluralization;
using Microsoft.Extensions.DependencyInjection;

namespace Main.Base.Instance;

public class ServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPluralizationService GetPluralizationService()
    {
        return _serviceProvider.GetRequiredService<IPluralizationService>();
    }
}
