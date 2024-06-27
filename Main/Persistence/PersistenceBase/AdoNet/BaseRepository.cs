using Main.Domain.AppBase;

namespace Main.Persistence.PersistenceBase.AdoNet;
public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
{
    public string TableName { get; set; }
    public List<string> GlobalFilters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public BaseRepository()
    {
        TableName = typeof(T).Name;
    }

    public int BulkDelete(string filter = "")
    {
        throw new NotImplementedException();
    }

    public int BulkInsert(List<T> entity)
    {
        throw new NotImplementedException();
    }

    public int BulkUpdate(List<T> entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(T entity)
    {
        throw new NotImplementedException();
    }

    public List<T> GetAll(string filter = "")
    {
        throw new NotImplementedException();
    }

    public T GetBy(string filter)
    {
        throw new NotImplementedException();
    }

    public T Insert(T entity)
    {
        throw new NotImplementedException();
    }

    public T Update(T entity)
    {
        throw new NotImplementedException();
    }
}
