namespace ELearning.Application.Instructors.Dtos;

public readonly record struct InstructorCourseDto(
    Guid Id,
    string Title,
    string Category,
    int EnrollmentsCount,
    string Status,
    DateTime CreatedAt,
    DateTime? PublishedDate
);