using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Courses.Commands;

/// <summary>
/// Command to update an existing course.
/// </summary>
public record UpdateCourseCommand(
    Guid CourseId,
    string Title,
    string Description,
    int CategoryId,
    int LevelId,
    decimal Price,
    int DurationHours,
    int DurationMinutes,
    bool IsFeatured
) : IRequest<Result>;