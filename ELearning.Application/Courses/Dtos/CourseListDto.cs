using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public sealed record CourseListDto(
    Guid Id,
    string Title,
    string Description,
    string InstructorName,
    string Category,
    string Level,
    decimal Price,
    decimal AverageRating,
    int NumberOfRatings,
    bool IsFeatured,
    string Duration,
    int EnrollmentsCount
) : IDto;
