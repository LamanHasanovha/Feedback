using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Globalization;

namespace Main.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly string _resourcesPath;
    private readonly ConcurrentDictionary<string, JObject> _localizationData = new ConcurrentDictionary<string, JObject>();

    public JsonStringLocalizer(IOptions<JsonLocalizationOptions> options)
    {
        _resourcesPath = options.Value.ResourcesPath;
    }

    private JObject GetLocalizationData(string culture)
    {
        return _localizationData.GetOrAdd(culture, _ =>
        {
            var filePath = Path.Combine(_resourcesPath, $"{culture}.json");
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                return JObject.Parse(jsonData);
            }
            return new JObject();
        });
    }

    public LocalizedString this[string name]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture.Name;
            var localizationData = GetLocalizationData(culture);
            var value = localizationData[name]?.ToString() ?? name;
            return new LocalizedString(name, value, value != name);
        }
    }

    public LocalizedString this[string name, params object[] arguments] => this[name];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        var localizationData = GetLocalizationData(culture);

        return localizationData.Properties().Select(prop => new LocalizedString(prop.Name, prop.Value.ToString(), true));
    }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        CultureInfo.CurrentUICulture = culture;
        return new JsonStringLocalizer(new OptionsWrapper<JsonLocalizationOptions>(new JsonLocalizationOptions { ResourcesPath = _resourcesPath }));
    }
}