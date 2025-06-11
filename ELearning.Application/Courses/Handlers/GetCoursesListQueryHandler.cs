using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.SharedKernel;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

public class GetCoursesListQueryHandler(
        ICourseRepository courseRepository,
        IMapper mapper)
    : IRequestHandler<GetCoursesListQuery, Result<PaginatedList<CourseListDto>>>
{
    public async Task<Result<PaginatedList<CourseListDto>>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        // In a real application, you would use Dapr for read operations here
        var courses = await courseRepository.SearchCoursesAsync(
            request.SearchTerm,
            request.ToPaginationParameters(),
            cancellationToken
            );

        var courseDtos = mapper.Map<List<CourseListDto>>(courses);

        var totalCount = await courseRepository.GetCoursesCountAsync();

        var paginatedList = new PaginatedList<CourseListDto>(
            courseDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(paginatedList);
    }
}