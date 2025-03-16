using ELearning.Application.Courses.Commands;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<CourseListDto>>> GetCourses(
        [FromQuery] string searchTerm,
        [FromQuery] int? categoryId,
        [FromQuery] int? levelId,
        [FromQuery] bool? isFeatured,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
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

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDetailDto>> GetCourse(Guid id)
    {
        var query = new GetCourseDetailQuery { CourseId = id };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(result.Error);
    }

    [HttpGet("featured")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CourseListDto>>> GetFeaturedCourses([FromQuery] int count = 5)
    {
        var query = new GetCoursesListQuery
        {
            IsFeatured = true,
            PageSize = count
        };

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value.Items);
        }

        return BadRequest(result.Error);
    }

    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CourseListDto>>> GetCoursesByCategory(int categoryId)
    {
        var query = new GetCoursesListQuery
        {
            CategoryId = categoryId
        };

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value.Items);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateCourse(CreateCourseCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            //return CreatedAtAction(nameof(GetCourse), new { id = result.Value }, result.Value);
            return CreatedAtAction(nameof(GetCourse), result);
        }

        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse(Guid id, UpdateCourseCommand command)
    {
        if (id != command.CourseId)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.Error.StartsWith("Course not found")
            ? NotFound(result.Error)
            : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var command = new DeleteCourseCommand { CourseId = id };
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(result.Error);
    }
}