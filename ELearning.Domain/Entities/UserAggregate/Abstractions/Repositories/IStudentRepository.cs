using ELearning.Domain.Entities.CourseAggregate;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IStudentRepository : IEntityFrameworkRepository<Student>
{
    Task<IReadOnlyList<Student>> GetByEnrolledCourseIdAsync(Guid courseId);

    Task<IReadOnlyList<Course>> GetEnrolledCoursesAsync(Guid studentId);

    Task<int> GetEnrolledStudentsCountByCourseIdAsync(Guid courseId);
}