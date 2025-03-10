using ELearning.Domain.Entities.CourseAggregate;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<IReadOnlyList<Student>> GetByEnrolledCourseIdAsync(Guid courseId);

    Task<IReadOnlyList<Course>> GetEnrolledCoursesAsync(Guid studentId);

    Task<int> GetEnrolledStudentsCountByCourseIdAsync(Guid courseId);
}