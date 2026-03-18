using ELearning.Application.Students.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

namespace ELearning.Application.Students.ReadModels;

public interface IStudentReadService : IReadRepository<StudentDto, Guid>
{
    Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId, CancellationToken cancellationToken = default);

    new Task<PaginatedList<StudentDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
}
