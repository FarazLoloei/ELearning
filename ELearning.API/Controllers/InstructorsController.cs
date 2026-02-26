using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Instructors.Queries;
using ELearning.Application.Submissions.Dtos;
using ELearning.Application.Submissions.Queries;
using ELearning.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class InstructorsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InstructorDto>>> GetInstructor(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInstructorProfileQuery { InstructorId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Instructor not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("{id:guid}/with-courses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InstructorCoursesDto>>> GetInstructorWithCourses(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInstructorCoursesQuery { InstructorId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Instructor not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("{id:guid}/pending-submissions")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<SubmissionDto>>>> GetPendingSubmissions(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPendingSubmissionsQuery
        {
            InstructorId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result);
    }
}
