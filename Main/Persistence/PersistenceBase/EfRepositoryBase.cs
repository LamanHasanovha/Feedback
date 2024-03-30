using Main.Domain.AppBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace Main.Persistence.PersistenceBase;

public class EfRepositoryBase<TDbContext, TEntity, TKey> : RepositoryBase<TEntity, TKey>, IRepositoryWithDbContext
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
{
    private readonly IDbContextProvider<TDbContext> _dbContextProvider;

    public virtual TDbContext Context => _dbContextProvider.GetDbContext();

    public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

    public virtual DbConnection Connection
    {
        get
        {
            DbConnection dbConnection = Context.Database.GetDbConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            return dbConnection;
        }
    }

    public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public override IQueryable<TEntity> GetAll()
    {
        return GetAllIncluding();
    }

    public override IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        IQueryable<TEntity> queryable = Queryable.AsQueryable(Table);
        if (!propertySelectors.IsNullOrEmpty())
        {
            foreach (Expression<Func<TEntity, object>> navigationPropertyPath in propertySelectors)
            {
                queryable = queryable.Include(navigationPropertyPath);
            }
        }

        return queryable;
    }

    public override async Task<List<TEntity>> GetAllListAsync()
    {
        return await GetAll().ToListAsync();
    }

    public override async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetAll().Where(predicate).ToListAsync();
    }

    public override async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetAll().SingleAsync(predicate);
    }

    public override async Task<TEntity> FirstOrDefaultAsync(TKey id)
    {
        return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
    }

    public override async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetAll().FirstOrDefaultAsync(predicate);
    }

    public override TEntity Insert(TEntity entity)
    {
        return Table.Add(entity).Entity;
    }

    public override Task<TEntity> InsertAsync(TEntity entity)
    {
        return Task.FromResult(Insert(entity));
    }

    public override TKey InsertAndGetId(TEntity entity)
    {
        entity = Insert(entity);
        if (entity.IsTransient())
        {
            Context.SaveChanges();
        }

        return entity.Id;
    }

    public override async Task<TKey> InsertAndGetIdAsync(TEntity entity)
    {
        entity = await InsertAsync(entity);
        if (entity.IsTransient())
        {
            await Context.SaveChangesAsync();
        }

        return entity.Id;
    }

    public override TKey InsertOrUpdateAndGetId(TEntity entity)
    {
        entity = InsertOrUpdate(entity);
        if (entity.IsTransient())
        {
            Context.SaveChanges();
        }

        return entity.Id;
    }

    public override async Task<TKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
    {
        entity = await InsertOrUpdateAsync(entity);
        if (entity.IsTransient())
        {
            await Context.SaveChangesAsync();
        }

        return entity.Id;
    }

    public override TEntity Update(TEntity entity)
    {
        AttachIfNot(entity);
        Context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public override Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity = Update(entity);
        return Task.FromResult(entity);
    }

    public override void Delete(TEntity entity)
    {
        AttachIfNot(entity);
        Table.Remove(entity);
    }

    public override void Delete(TKey id)
    {
        TEntity fromChangeTrackerOrNull = GetFromChangeTrackerOrNull(id);
        if (fromChangeTrackerOrNull != null)
        {
            Delete(fromChangeTrackerOrNull);
            return;
        }

        fromChangeTrackerOrNull = FirstOrDefault(id);
        if (fromChangeTrackerOrNull != null)
        {
            Delete(fromChangeTrackerOrNull);
        }
    }

    public override async Task<int> CountAsync()
    {
        return await GetAll().CountAsync();
    }

    public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetAll().Where(predicate).CountAsync();
    }

    public override async Task<long> LongCountAsync()
    {
        return await GetAll().LongCountAsync();
    }

    public override async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetAll().Where(predicate).LongCountAsync();
    }

    protected virtual void AttachIfNot(TEntity entity)
    {
        if (Enumerable.FirstOrDefault(Context.ChangeTracker.Entries(), (EntityEntry ent) => ent.Entity == entity) == null)
        {
            Table.Attach(entity);
        }
    }

    public DbContext GetDbContext()
    {
        return Context;
    }

    private TEntity GetFromChangeTrackerOrNull(TKey id)
    {
        return Enumerable.FirstOrDefault(Context.ChangeTracker.Entries(), (EntityEntry entry) => entry.Entity is TEntity && EqualityComparer<TKey>.Default.Equals(id, (entry.Entity as TEntity).Id))?.Entity as TEntity;
    }
}

public class EfRepositoryBase<TEntity, TKey>(IDbContextProvider<DbContext> dbContextProvider) : EfRepositoryBase<DbContext, TEntity, TKey>(dbContextProvider)
    where TEntity : class, IEntity<TKey>
{
}