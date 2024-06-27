using Main.Domain.AppBase;

namespace Main.Persistence.PersistenceBase.AdoNet;

public interface IBaseRepository<T> where T : class, IEntity
{
    string TableName { get; set; }

    List<string> GlobalFilters { get; set; }

    List<T> GetAll(string filter = "");

    T GetBy(string filter);

    T Insert(T entity);

    T Update(T entity);

    void Delete(T entity);

    int BulkDelete(string filter = "");

    int BulkInsert(List<T> entity);

    int BulkUpdate(List<T> entity);
}
