using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Main.Localization;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IOptions<JsonLocalizationOptions> _options;

    public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> options)
    {
        _options = options;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(_options);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(_options);
    }
}
