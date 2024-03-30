using Main.Domain.AppBase;
using Main.Persistence.Exceptions;
using System.Linq.Expressions;

namespace Main.Persistence.PersistenceBase;

public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    static RepositoryBase()
    {

    }

    public abstract IQueryable<TEntity> GetAll();

    public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return GetAll();
    }

    public virtual List<TEntity> GetAllList()
    {
        return Enumerable.ToList(GetAll());
    }

    public virtual Task<List<TEntity>> GetAllListAsync()
    {
        return Task.FromResult(GetAllList());
    }

    public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
    {
        return Enumerable.ToList(GetAll().Where(predicate));
    }

    public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(GetAllList(predicate));
    }

    public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
    {
        return queryMethod(GetAll());
    }

    public virtual TEntity Get(TKey id)
    {
        return FirstOrDefault(id) ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public virtual async Task<TEntity> GetAsync(TKey id)
    {
        return (await FirstOrDefaultAsync(id)) ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Single(predicate);
    }

    public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(Single(predicate));
    }

    public virtual TEntity FirstOrDefault(TKey id)
    {
        return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
    }

    public virtual Task<TEntity> FirstOrDefaultAsync(TKey id)
    {
        return Task.FromResult(FirstOrDefault(id));
    }

    public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().FirstOrDefault(predicate);
    }

    public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(FirstOrDefault(predicate));
    }

    public virtual TEntity Load(TKey id)
    {
        return Get(id);
    }

    public abstract TEntity Insert(TEntity entity);

    public virtual Task<TEntity> InsertAsync(TEntity entity)
    {
        return Task.FromResult(Insert(entity));
    }

    public virtual TKey InsertAndGetId(TEntity entity)
    {
        return Insert(entity).Id;
    }

    public virtual Task<TKey> InsertAndGetIdAsync(TEntity entity)
    {
        return Task.FromResult(InsertAndGetId(entity));
    }

    public virtual TEntity InsertOrUpdate(TEntity entity)
    {
        if (!entity.IsTransient())
            return Update(entity);

        return Insert(entity);
    }

    public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
    {
        return (!entity.IsTransient()) ? (await UpdateAsync(entity)) : (await InsertAsync(entity));
    }

    public virtual TKey InsertOrUpdateAndGetId(TEntity entity)
    {
        return InsertOrUpdate(entity).Id;
    }

    public virtual Task<TKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
    {
        return Task.FromResult(InsertOrUpdateAndGetId(entity));
    }

    public abstract TEntity Update(TEntity entity);

    public virtual Task<TEntity> UpdateAsync(TEntity entity)
    {
        return Task.FromResult(Update(entity));
    }

    public virtual TEntity Update(TKey id, Action<TEntity> updateAction)
    {
        TEntity val = Get(id);
        updateAction(val);
        return val;
    }

    public virtual async Task<TEntity> UpdateAsync(TKey id, Func<TEntity, Task> updateAction)
    {
        TEntity entity = await GetAsync(id);
        await updateAction(entity);
        return entity;
    }

    public abstract void Delete(TEntity entity);

    public virtual Task DeleteAsync(TEntity entity)
    {
        Delete(entity);
        return Task.FromResult(0);
    }

    public abstract void Delete(TKey id);

    public virtual Task DeleteAsync(TKey id)
    {
        Delete(id);
        return Task.FromResult(0);
    }

    public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
    {
        foreach (TEntity item in Enumerable.ToList(GetAll().Where(predicate)))
        {
            Delete(item);
        }
    }

    public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        Delete(predicate);
        return Task.FromResult(0);
    }

    public virtual int Count()
    {
        return GetAll().Count();
    }

    public virtual Task<int> CountAsync()
    {
        return Task.FromResult(Count());
    }

    public virtual int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Where(predicate).Count();
    }

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(Count(predicate));
    }

    public virtual long LongCount()
    {
        return GetAll().LongCount();
    }

    public virtual Task<long> LongCountAsync()
    {
        return Task.FromResult(LongCount());
    }

    public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
    {
        return GetAll().Where(predicate).LongCount();
    }

    public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Task.FromResult(LongCount(predicate));
    }

    protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TKey id)
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
        return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.PropertyOrField(parameterExpression, "Id"), Expression.Constant(id, typeof(TKey))), new ParameterExpression[1] { parameterExpression });
    }

}
