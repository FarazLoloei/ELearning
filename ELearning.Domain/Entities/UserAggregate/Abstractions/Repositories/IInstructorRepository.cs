using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IInstructorRepository : IRepository<Instructor>
{
    Task<IReadOnlyList<Instructor>> GetTopInstructorsAsync(int count);

    Task<int> GetTotalStudentsCountByInstructorIdAsync(Guid instructorId);

    Task<decimal> GetAverageRatingByInstructorIdAsync(Guid instructorId);
}