using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;

namespace Main.Persistence.Authorization;

public class UserRepository(IDbContextProvider<FeedbackCoreContext> dbContextProvider) : EfRepositoryBase<User, int>(dbContextProvider), IUserRepository
{
}
