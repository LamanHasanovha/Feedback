using Application.Students.Dto;
using Core.Entities;
using Main.Application.ApplicationBase;
using Persistence.Students;

namespace Application.Students;

public class StudentService : ApplicationBaseService, IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public List<StudentDto> GetStudents()
    {
        var data = _studentRepository.GetAll();

        var data1 = _studentRepository.GetBy("Id = 2");
        
        return Mapper.Map<Student, StudentDto>(data);
    }
}
