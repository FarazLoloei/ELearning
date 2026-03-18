using ELearning.Application.Instructors.ReadModels;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IInstructorReadRepository : IReadRepository<InstructorReadModel, Guid>
{
    Task<InstructorWithCoursesReadModel?> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken = default);

    Task<int> GetTotalStudentCountAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<decimal> GetAverageRatingAsync(Guid instructorId, CancellationToken cancellationToken);
}