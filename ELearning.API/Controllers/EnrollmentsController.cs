using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.Application.Enrollments.Commands;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class EnrollmentsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentDetailDto>>> GetEnrollment(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetEnrollmentDetailQuery { EnrollmentId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object?>>> CreateEnrollment(
        CreateEnrollmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequestResponse<object?>(result.Error);
        }

        return CreatedResponse();
    }

    [HttpPut("{id:guid}/status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> UpdateEnrollmentStatus(
        Guid id,
        UpdateEnrollmentStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.EnrollmentId)
        {
            return BadRequestResponse<object?>("Route id does not match payload EnrollmentId.");
        }

        var result = await mediator.Send(command, cancellationToken);
        return FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }
}
