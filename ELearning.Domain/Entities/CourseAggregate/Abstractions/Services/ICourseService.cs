using ELearning.Domain.ValueObjects;

namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;

public interface ICourseService
{
    Task<bool> CanPublishCourseAsync(Course course);

    Task UpdateCourseRatingAsync(Course course, Rating newRating, Rating? oldRating = null);

    Task<bool> IsCourseTitleUniqueForInstructorAsync(string title, Guid instructorId, Guid? excludeCourseId = null);

    Task<decimal> CalculateAverageCompletionTimeAsync(Guid courseId);
}