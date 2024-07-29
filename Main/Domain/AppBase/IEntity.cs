namespace Main.Domain.AppBase;

public interface IEntity<TKey>
{
    TKey Id { get; set; }

    bool IsTransient();
}

public interface IEntity : IEntity<int>
{

}
