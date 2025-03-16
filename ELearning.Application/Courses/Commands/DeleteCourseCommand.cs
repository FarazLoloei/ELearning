using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Courses.Commands;

/// <summary>
/// Command to delete a course
/// </summary>
public class DeleteCourseCommand : IRequest<Result>
{
    public Guid CourseId { get; set; }
}