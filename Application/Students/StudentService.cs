using Application.Students.Dto;
using Interfaces.Students.Application.Dto;
using Interfaces.Students.Persistence;

namespace Application.Students;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public StudentDto GetStudent()
    {
        var data = _studentRepository.Get(1);
        return new StudentDto
        {
            Id = data.Id,
            Name = data.Name
        };
    }
}
