using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Students;

namespace ELearning.Infrastructure.Dapr.Abstraction;

// Interface for User read service
public interface IUserReadService
{
    Task<StudentDto> GetStudentByIdAsync(Guid id);

    Task<InstructorDto> GetInstructorByIdAsync(Guid id);

    Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId);

    Task<InstructorCoursesDto> GetInstructorWithCoursesAsync(Guid instructorId);
}