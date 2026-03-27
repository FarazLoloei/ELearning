// <copyright file="GetCourseReviewsQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using MediatR;

public sealed class GetCourseReviewsQueryHandler(
        ICourseRepository courseRepository,
        ICourseReadRepository courseReadRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetCourseReviewsQuery, Result<IReadOnlyList<ReviewDto>>>
{
    public async Task<Result<IReadOnlyList<ReviewDto>>> Handle(GetCourseReviewsQuery request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        var canViewUnpublishedCourse =
            currentUserService.IsAuthenticated &&
            currentUserService.UserId is Guid currentUserId &&
            (course.IsOwnedBy(currentUserId) || currentUserService.IsInRole("Admin"));

        if (!course.IsPubliclyVisible() && !canViewUnpublishedCourse)
        {
            throw new NotFoundException(nameof(Course), request.CourseId);
        }

        var reviews = await courseReadRepository.GetReviewsByCourseIdAsync(request.CourseId, cancellationToken);

        return Result.Success<IReadOnlyList<ReviewDto>>(
        [.. reviews.Select(review => new ReviewDto(
            review.Id,
            review.StudentName,
            review.Rating,
            review.Comment,
            review.CreatedAt))]);
    }
}
