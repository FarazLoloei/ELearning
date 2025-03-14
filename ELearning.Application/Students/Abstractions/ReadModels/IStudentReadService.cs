using ELearning.Application.Students.Dtos;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Abstractions.ReadModels;

public interface IStudentReadService : IReadRepository<StudentDto, Guid>
{
    Task<StudentDto> GetStudentByIdAsync(Guid id);

    Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId);
}