using Main.Domain.AppBase;

namespace Main.Persistence.PersistenceBase.AdoNet;

public interface IBaseRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
{
    string ConnectionString { get; }

    string TableName { get; }

    List<string> GlobalFilters { get; set; }
    
    public bool IsIdIdentity { get; }

    List<TEntity> GetAll(string filter = "");

    TEntity GetBy(string filter);

    TEntity Insert(TEntity entity);

    TEntity Update(TEntity entity);

    void Delete(TEntity entity);

    int BulkDelete(string filter = "");

    int BulkInsert(List<TEntity> entities);

    int BulkUpdate(List<TEntity> entities);
}
