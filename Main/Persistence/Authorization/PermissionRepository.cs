using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;

namespace Main.Persistence.Authorization;

public class PermissionRepository(IDbContextProvider<FeedbackCoreContext> dbContextProvider) : EfRepositoryBase<Permission, int>(dbContextProvider), IPermissionRepository
{
}
