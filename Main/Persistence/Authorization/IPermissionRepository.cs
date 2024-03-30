using Main.Domain.Authorization;
using Main.Persistence.PersistenceBase;

namespace Main.Persistence.Authorization;

public interface IPermissionRepository : IRepository<Permission, int>
{
}
