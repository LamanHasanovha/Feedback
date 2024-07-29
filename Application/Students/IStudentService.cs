using Application.Students.Dto;
using Main.Application.ApplicationBase;

namespace Application.Students;

public interface IStudentService : IApplicationBaseService
{
    List<StudentDto> GetStudents();
}
