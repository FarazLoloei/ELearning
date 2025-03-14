using ELearning.Application.Instructors.Dtos;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Instructors.Abstractions.ReadModels;

public interface IInstructorReadService : IReadRepository<InstructorDto, Guid>
{
    Task<InstructorDto> GetInstructorByIdAsync(Guid id);

    Task<InstructorCoursesDto> GetInstructorWithCoursesAsync(Guid instructorId);
}