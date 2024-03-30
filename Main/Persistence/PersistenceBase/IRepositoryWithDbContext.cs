using Microsoft.EntityFrameworkCore;

namespace Main.Persistence.PersistenceBase;

public interface IRepositoryWithDbContext
{
    DbContext GetDbContext();
}
