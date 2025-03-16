using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Courses.Commands;

/// <summary>
/// Command to update a course
/// </summary>
public class UpdateCourseCommand : IRequest<Result>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public int LevelId { get; set; }
    public decimal Price { get; set; }
    public int DurationHours { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsFeatured { get; set; }
}