using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;

namespace Main.Persistence.Authorization;

public class UserPermissionRepository(IDbContextProvider<FeedbackCoreContext> dbContextProvider) : EfRepositoryBase<UserPermission, int>(dbContextProvider), IUserPermissionRepository
{
}
