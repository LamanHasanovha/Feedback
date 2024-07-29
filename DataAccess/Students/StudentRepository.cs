using Core.Entities;
using Main.Persistence.PersistenceBase.AdoNet;
using Microsoft.Extensions.Configuration;

namespace Persistence.Students;

public class StudentRepository : BaseRepository<Student, int>, IStudentRepository
{
    public StudentRepository(IConfiguration configuration) : base(configuration)
    {
    }
}
