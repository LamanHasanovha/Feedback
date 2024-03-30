using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;
using Microsoft.EntityFrameworkCore;

namespace Main.Persistence.Authorization;

public class UserRepository(IDbContextProvider<DbContext> dbContextProvider) : EfRepositoryBase<User, int>(dbContextProvider), IUserRepository
{
}
