using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using MediatR;

namespace ELearning.Application.Courses.Queries;

public record GetCourseDetailQuery : IRequest<Result<CourseDto>>
{
    public Guid CourseId { get; set; }
}
