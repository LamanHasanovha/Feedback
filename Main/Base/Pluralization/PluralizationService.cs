using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Main.Base.Pluralization;

public class PluralizationService : IPluralizationService
{
    private readonly Dictionary<string, string> IrregularPlurals = new Dictionary<string, string>
    {
        { "child", "children" },
        { "person", "people" },
        { "man", "men" },
        { "woman", "women" },
        { "mouse", "mice" },
        { "goose", "geese" },
        { "foot", "feet" },
        { "tooth", "teeth" },
        { "ox", "oxen" }
    };

    private readonly HashSet<string> UncountableWords = new HashSet<string>
    {
        "advice", "bread", "information", "equipment", "furniture", "homework", "jewelry", "luggage",
        "news", "money", "rice", "series", "sheep", "species", "deer", "fish", "aircraft", "water",
        "sugar", "music", "art", "baggage", "clothing", "commerce", "education", "electricity",
        "experience", "fruit", "happiness", "homework", "imagination", "laughter", "leisure",
        "literature", "luggage", "machinery", "mathematics", "meat", "milk", "money", "mud", "news",
        "patience", "pollution", "progress", "rain", "reliability", "salt", "shopping", "snow",
        "software", "stamina", "strength", "traffic", "transportation", "trust", "violence", "weather",
        "wood", "wool", "work"
    };

    private readonly List<Rule> PluralizationRules = new List<Rule>
    {
        new Rule("$", "s"),
        new Rule("s$", "ses"),
        new Rule("x$", "xes"),
        new Rule("z$", "zes"),
        new Rule("ch$", "ches"),
        new Rule("sh$", "shes"),
        new Rule("y$", "ies", @"[aeiou]y$"),
        new Rule("f$", "ves", @"[^aeiou]f$"),
        new Rule("fe$", "ves", @"[^aeiou]fe$"),
        new Rule("us$", "i"),
        new Rule("is$", "es"),
        new Rule("on$", "a"),
        new Rule("um$", "a")
    };

    private readonly List<Rule> SingularizationRules = new List<Rule>
    {
        new Rule("s$", ""),
        new Rule("ses$", "s"),
        new Rule("xes$", "x"),
        new Rule("zes$", "z"),
        new Rule("ches$", "ch"),
        new Rule("shes$", "sh"),
        new Rule("ies$", "y", @"[aeiou]ies$"),
        new Rule("ves$", "f", @"[^aeiou]ves$"),
        new Rule("ves$", "fe", @"[^aeiou]ves$"),
        new Rule("i$", "us"),
        new Rule("es$", "is"),
        new Rule("a$", "on"),
        new Rule("a$", "um")
    };

    public string Pluralize(string singular)
    {
        if (string.IsNullOrEmpty(singular))
            return singular;

        if (UncountableWords.Contains(singular.ToLower()))
            return singular;

        if (IrregularPlurals.ContainsKey(singular.ToLower()))
            return IrregularPlurals[singular.ToLower()];

        foreach (var rule in PluralizationRules)
        {
            if (rule.IsMatch(singular))
            {
                return rule.Apply(singular);
            }
        }

        return singular + "s";
    }

    public string Singularize(string plural)
    {
        if (string.IsNullOrEmpty(plural))
            return plural;

        if (UncountableWords.Contains(plural.ToLower()))
            return plural;

        var irregularSingular = IrregularPlurals.FirstOrDefault(x => x.Value.Equals(plural, StringComparison.OrdinalIgnoreCase)).Key;
        if (irregularSingular != null)
            return irregularSingular;

        foreach (var rule in SingularizationRules)
        {
            if (rule.IsMatch(plural))
            {
                return rule.Apply(plural);
            }
        }

        return plural.TrimEnd('s');
    }
}

public class Rule
{
    private readonly Regex _regex;
    private readonly string _replacement;
    private readonly Regex _exception;

    public Rule(string pattern, string replacement, string exceptionPattern = null)
    {
        _regex = new Regex(pattern, RegexOptions.IgnoreCase);
        _replacement = replacement;
        if (exceptionPattern != null)
        {
            _exception = new Regex(exceptionPattern, RegexOptions.IgnoreCase);
        }
    }

    public bool IsMatch(string word)
    {
        return _exception == null ? _regex.IsMatch(word) : _regex.IsMatch(word) && !_exception.IsMatch(word);
    }

    public string Apply(string word)
    {
        return _regex.Replace(word, _replacement);
    }
}