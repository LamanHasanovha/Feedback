using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;

namespace Main.Persistence.Authorization;

public interface IUserRepository : IRepository<User, int>
{
}
