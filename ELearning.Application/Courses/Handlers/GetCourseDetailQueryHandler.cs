﻿using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

public class GetCourseDetailQueryHandler(
        ICourseRepository courseRepository,
        IMapper mapper)
    : IRequestHandler<GetCourseDetailQuery, Result<CourseDetailDto>>
{
    public async Task<Result<CourseDetailDto>> Handle(GetCourseDetailQuery request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId);

        if (course is null)
            throw new NotFoundException(nameof(Course), request.CourseId);

        var courseDto = mapper.Map<CourseDetailDto>(course);

        // Optionally enrich with additional data not handled by AutoMapper

        return Result.Success(courseDto);
    }
}