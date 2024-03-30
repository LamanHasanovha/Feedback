using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;
using Microsoft.EntityFrameworkCore;

namespace Main.Persistence.Authorization;

public class PermissionRepository(IDbContextProvider<DbContext> dbContextProvider) : EfRepositoryBase<Permission, int>(dbContextProvider), IPermissionRepository
{
}
