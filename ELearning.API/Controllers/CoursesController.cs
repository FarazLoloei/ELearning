// <copyright file="CoursesController.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Controllers;

using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.API.Facades;
using ELearning.Application.Courses.Commands;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class CoursesController(IApiFacade apiFacade) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CourseListDto>>>> GetCourses(
        [FromQuery] string? searchTerm,
        [FromQuery] int? categoryId,
        [FromQuery] int? levelId,
        [FromQuery] bool? isFeatured,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesListQuery
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            LevelId = levelId,
            IsFeatured = isFeatured,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var result = await apiFacade.SendAsync(query, cancellationToken);
        return this.FromResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetCourse(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCourseDetailQuery { CourseId = id };
        var result = await apiFacade.SendAsync(query, cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("{id:guid}/reviews")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ReviewDto>>>> GetCourseReviews(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new GetCourseReviewsQuery(id), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("featured")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CourseListDto>>>> GetFeaturedCourses(
        [FromQuery] int count = 5,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesListQuery
        {
            IsFeatured = true,
            PageSize = count,
        };

        var result = await apiFacade.SendAsync(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.BadRequestResponse<List<CourseListDto>>(result.Error);
        }

        return this.Ok(ApiResponse<List<CourseListDto>>.Success(result.Value.Items.ToList()));
    }

    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CourseListDto>>>> GetCoursesByCategory(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesListQuery
        {
            CategoryId = categoryId,
        };

        var result = await apiFacade.SendAsync(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.BadRequestResponse<List<CourseListDto>>(result.Error);
        }

        return this.Ok(ApiResponse<List<CourseListDto>>.Success(result.Value.Items.ToList()));
    }

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object?>>> CreateCourse(
        CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.BadRequestResponse<object?>(result.Error);
        }

        return this.CreatedResponse();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> UpdateCourse(
        Guid id,
        UpdateCourseCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.CourseId)
        {
            return this.BadRequestResponse<object?>("Route id does not match payload CourseId.");
        }

        var result = await apiFacade.SendAsync(command, cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new DeleteCourseCommand(id), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost("{id:guid}/submit-for-review")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> SubmitForReview(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new SubmitCourseForReviewCommand(id), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost("{id:guid}/approve-publication")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> ApprovePublication(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new ApproveCoursePublicationCommand(id), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost("{id:guid}/reject-publication")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> RejectPublication(
        Guid id,
        RejectCoursePublicationCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.CourseId)
        {
            return this.BadRequestResponse<object?>("Route id does not match payload CourseId.");
        }

        var result = await apiFacade.SendAsync(command, cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> Archive(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new ArchiveCourseCommand(id), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }
}
