using Core.Entities;
using Main.Persistence.PersistenceBase;

namespace Interfaces.Students.Persistence;

public interface IStudentRepository : IRepository<Student, int>
{
}
