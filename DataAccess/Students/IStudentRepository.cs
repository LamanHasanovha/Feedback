using Core.Entities;
using Main.Persistence.PersistenceBase.AdoNet;

namespace Persistence.Students;

public interface IStudentRepository : IBaseRepository<Student, int>
{
}
