using Core.Entities;
using Interfaces.Students.Persistence;
using Main.Persistence.PersistenceBase;
using Persistence.Contexts;

namespace Persistence.Students;

public class StudentRepository(IDbContextProvider<FeedbackContext> dbContextProvider) : EfRepositoryBase<FeedbackContext, Student, int>(dbContextProvider), IStudentRepository
{
}
