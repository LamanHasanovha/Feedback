using Microsoft.EntityFrameworkCore;

namespace Main.Persistence.PersistenceBase;

public interface IDbContextProvider<out TDbContext> where TDbContext : DbContext
{
    TDbContext GetDbContext();
}
