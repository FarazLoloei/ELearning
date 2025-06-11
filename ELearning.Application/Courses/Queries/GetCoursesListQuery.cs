using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using MediatR;

namespace ELearning.Application.Courses.Queries;

public record GetCoursesListQuery : IRequest<Result<PaginatedList<CourseListDto>>>, IPaginatable
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchTerm { get; set; }

    public int? CategoryId { get; set; }

    public int? LevelId { get; set; }

    public bool? IsFeatured { get; set; }
}