using System.ComponentModel.DataAnnotations;

namespace Main.Domain.AppBase;

public class Entity<TKey> : IEntity<TKey>
{
    [Key]
    public TKey Id { get; set; }

    public bool IsTransient()
    {
        if (EqualityComparer<TKey>.Default.Equals(Id, default(TKey)))
            return true;

        if (typeof(TKey) == typeof(int))
            return Convert.ToInt32(Id) <= 0;

        if (typeof(TKey) == typeof(long))
            return Convert.ToInt64(Id) <= 0;

        return false;
    }
}

public class Entity : Entity<int>
{
}
