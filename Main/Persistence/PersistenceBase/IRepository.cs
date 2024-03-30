using Main.Domain.AppBase;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Main.Persistence.PersistenceBase;

public interface IRepository
{
}

public interface IRepository<TContext, TEntity, TKey> : IRepository
    where TEntity : class, IEntity<TKey>
    where TContext : DbContext
{
    IQueryable<TEntity> GetAll();
    
    IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);
    
    List<TEntity> GetAllList();
    
    Task<List<TEntity>> GetAllListAsync();
    
    List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
    
    Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
    
    T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
    
    TEntity Get(TKey id);
    
    Task<TEntity> GetAsync(TKey id);
    
    TEntity Single(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
    
    TEntity FirstOrDefault(TKey id);
    
    Task<TEntity> FirstOrDefaultAsync(TKey id);
    
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    
    TEntity Load(TKey id);
    
    TEntity Insert(TEntity entity);
    
    Task<TEntity> InsertAsync(TEntity entity);
    
    TKey InsertAndGetId(TEntity entity);
    
    Task<TKey> InsertAndGetIdAsync(TEntity entity);
    
    TEntity InsertOrUpdate(TEntity entity);
    
    Task<TEntity> InsertOrUpdateAsync(TEntity entity);
    
    TKey InsertOrUpdateAndGetId(TEntity entity);
    
    Task<TKey> InsertOrUpdateAndGetIdAsync(TEntity entity);
    
    TEntity Update(TEntity entity);
    
    Task<TEntity> UpdateAsync(TEntity entity);
    
    TEntity Update(TKey id, Action<TEntity> updateAction);
    
    Task<TEntity> UpdateAsync(TKey id, Func<TEntity, Task> updateAction);
    
    void Delete(TEntity entity);
    
    Task DeleteAsync(TEntity entity);
    
    void Delete(TKey id);
    
    Task DeleteAsync(TKey id);
    
    void Delete(Expression<Func<TEntity, bool>> predicate);
    
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    
    int Count();
    
    Task<int> CountAsync();
    int Count(Expression<Func<TEntity, bool>> predicate);
    
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    
    long LongCount();
    
    Task<long> LongCountAsync();
    
    long LongCount(Expression<Func<TEntity, bool>> predicate);
    
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
}

//public interface IRepository<TContext, TEntity> : IRepository<TContext, TEntity, int>
//    where TEntity : class, IEntity<int>
//    where TContext : DbContext
//{
//}

public interface IRepository<TEntity, TKey> : IRepository<DbContext, TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{

}
