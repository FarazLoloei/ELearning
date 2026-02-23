using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Instructors.Dtos;

public sealed record InstructorCourseDto(
    Guid Id,
    string Title,
    string Category,
    int EnrollmentsCount,
    string Status,
    DateTime CreatedAt,
    DateTime? PublishedDate
) : IDto;
