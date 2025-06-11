using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Courses.Commands;

/// <summary>
/// Command to create a new course.
/// </summary>
public record CreateCourseCommand(
    string Title,
    string Description,
    int CategoryId,
    int LevelId,
    decimal Price,
    int DurationHours,
    int DurationMinutes
) : IRequest<Result>;