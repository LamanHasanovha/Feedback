using Main.Domain.AppBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Main.Persistence.PersistenceBase;

public class BaseContext(DbContextOptions options) : DbContext(options)
{
    public long? CurrentUserId { get; set; }

    public override int SaveChanges()
    {
        Apply();

        return base.SaveChanges();
    }

    protected virtual void Apply()
    {
        ConfigureCreationAudit(ChangeTracker.Entries<IHasCreationTime>());

        ConfigureModificationAudit(ChangeTracker.Entries<IHasModificationTime>());

        ConfigureDeletionAudit(ChangeTracker.Entries<ISoftDelete>());
    }

    protected virtual void ConfigureCreationAudit(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.Entity is not IHasCreationTime hasCreationTimeEntity) continue;

            hasCreationTimeEntity.CreationTime = DateTime.Now;

            if (entry.Entity is ICreationAudited creationAuditedEntity)
                creationAuditedEntity.CreatorUserId = CurrentUserId;
        }
    }

    protected virtual void ConfigureModificationAudit(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.Entity is not IHasModificationTime hasModificationTimeEntity) continue;

            hasModificationTimeEntity.LastModificationTime = DateTime.Now;

            if (entry is IModificationAudited modificationAuditedEntity)
                modificationAuditedEntity.LastModifierId = CurrentUserId;
        }
    }

    protected virtual void ConfigureDeletionAudit(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.Entity is not ISoftDelete softDeleteEntity) continue;

            softDeleteEntity.IsDeleted = true;

            if (entry is IHasDeletionTime hasDeletionTimeEntity)
                hasDeletionTimeEntity.DeletionTime = DateTime.Now;

            if (entry is IDeletionAudited deletionAuditedEntity)
                deletionAuditedEntity.DeleterUserId = CurrentUserId;
        }
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Apply();

        return base.SaveChangesAsync(cancellationToken);
    }


}
