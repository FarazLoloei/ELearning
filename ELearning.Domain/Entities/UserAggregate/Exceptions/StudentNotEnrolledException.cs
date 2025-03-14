using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.UserAggregate.Exceptions;

public class StudentNotEnrolledException : DomainException
{
    public StudentNotEnrolledException(Guid studentId, Guid courseId)
        : base($"Student with ID {studentId} is not enrolled in course with ID {courseId}.")
    {
    }
}