using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;
using Microsoft.EntityFrameworkCore;

namespace Main.Persistence.Authorization;

public class UserPermissionRepository(IDbContextProvider<DbContext> dbContextProvider) : EfRepositoryBase<UserPermission, int>(dbContextProvider), IUserPermissionRepository
{
}
