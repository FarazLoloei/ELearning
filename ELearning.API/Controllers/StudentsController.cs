using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using ELearning.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class StudentsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> GetStudent(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStudentProfileQuery { StudentId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Student not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("{id:guid}/progress")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentProgressDto>>> GetStudentProgress(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStudentProgressQuery { StudentId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Student", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("{id:guid}/enrollments")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<EnrollmentDto>>>> GetStudentEnrollments(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentEnrollmentsQuery
        {
            StudentId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result);
    }
}
