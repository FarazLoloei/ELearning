using ELearning.Application.Enrollments.Commands;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentDetailDto>> GetEnrollment(Guid id)
    {
        var query = new GetEnrollmentDetailQuery { EnrollmentId = id };
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateEnrollment(CreateEnrollmentCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetEnrollment), result);
            //return CreatedAtAction(nameof(GetEnrollment), new { id = result.Value }, result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPut("{id}/status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEnrollmentStatus(Guid id, UpdateEnrollmentStatusCommand command)
    {
        if (id != command.EnrollmentId)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.Error.StartsWith("Enrollment not found")
            ? NotFound(result.Error)
            : BadRequest(result.Error);
    }
}