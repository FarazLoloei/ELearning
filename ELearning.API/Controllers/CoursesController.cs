using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.Application.Courses.Commands;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class CoursesController(IMediator mediator) : ApiControllerBase
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
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetCourse(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCourseDetailQuery { CourseId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
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
            PageSize = count
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequestResponse<List<CourseListDto>>(result.Error);
        }

        return Ok(ApiResponse<List<CourseListDto>>.Success(result.Value.Items.ToList()));
    }

    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CourseListDto>>>> GetCoursesByCategory(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesListQuery
        {
            CategoryId = categoryId
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequestResponse<List<CourseListDto>>(result.Error);
        }

        return Ok(ApiResponse<List<CourseListDto>>.Success(result.Value.Items.ToList()));
    }

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object?>>> CreateCourse(
        CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequestResponse<object?>(result.Error);
        }

        return CreatedResponse();
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
            return BadRequestResponse<object?>("Route id does not match payload CourseId.");
        }

        var result = await mediator.Send(command, cancellationToken);
        return FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteCourse(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteCourseCommand(id), cancellationToken);
        return FromResult(result, error => error.StartsWith("Course not found", StringComparison.OrdinalIgnoreCase));
    }
}
