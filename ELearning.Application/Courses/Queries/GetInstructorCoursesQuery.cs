using ELearning.Application.Common.Model;
using ELearning.Application.Instructors.Dtos;
using MediatR;

namespace ELearning.Application.Courses.Queries;

/// <summary>
/// Query to get instructor with their courses
/// </summary>
public record GetInstructorCoursesQuery : IRequest<Result<InstructorCoursesDto>>
{
    public Guid InstructorId { get; set; }
}