using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Main.Application.Mapper;

public class FeedbackMapper : IMapper
{
    private readonly ConcurrentDictionary<(Type Source, Type Target), Delegate> _mapFuncs = new();

    public TTarget Map<TCurrent, TTarget>(TCurrent source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var mapFunc = (Func<TCurrent, TTarget>)_mapFuncs.GetOrAdd((typeof(TCurrent), typeof(TTarget)), _ =>
        {
            var parameterExpression = Expression.Parameter(typeof(TCurrent), "source");
            var bindings = typeof(TTarget)
                .GetProperties()
                .Where(targetProp => targetProp.CanWrite)
                .Select(targetProp =>
                {
                    var sourceProp = typeof(TCurrent).GetProperty(targetProp.Name);
                    if (sourceProp == null) return null;

                    var sourceValue = Expression.Property(parameterExpression, sourceProp);
                    return Expression.Bind(targetProp, sourceValue);
                })
                .Where(binding => binding != null)
                .Cast<MemberBinding>()
                .ToArray();

            var memberInit = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            var lambda = Expression.Lambda<Func<TCurrent, TTarget>>(memberInit, parameterExpression);
            return lambda.Compile();
        });

        return mapFunc(source);
    }

    public List<TTarget> Map<TCurrent, TTarget>(List<TCurrent> source)
    {
        return source == null
            ? throw new ArgumentNullException(nameof(source))
            : source.Select(Map<TCurrent, TTarget>).ToList();
    }

    public async Task<List<TTarget>> MapAsync<TCurrent, TTarget>(List<TCurrent> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var results = await Task.WhenAll(source.Select(item => Task.Run(() => Map<TCurrent, TTarget>(item)))).ConfigureAwait(false);
        return results.ToList();
    }

    #region OldVersion

    [Obsolete]
    public TTarget MapOld<TCurrent, TTarget>(TCurrent source)
    {
        var data = Activator.CreateInstance<TTarget>();

        var targetProperties = source.GetType().GetProperties();
        var destinationProperties = data.GetType().GetProperties();

        foreach (var dp in destinationProperties)
        {
            var val = targetProperties.FirstOrDefault(p => p.Name == dp.Name)?.GetValue(source);
            dp.SetValue(data, val);
        }

        return data;
    }

    [Obsolete]
    public List<TTarget> MapOld<TCurrent, TTarget>(List<TCurrent> source)
    {
        var result = new List<TTarget>();

        foreach (var item in source)
        {
            result.Add(MapOld<TCurrent, TTarget>(item));
        }

        return result;
    }


    #endregion
}
