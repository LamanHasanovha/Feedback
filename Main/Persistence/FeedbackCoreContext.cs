using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;
using Microsoft.EntityFrameworkCore;

namespace Main.Persistence;

public class FeedbackCoreContext(DbContextOptions<FeedbackCoreContext> options) : BaseContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id);
    }
}
