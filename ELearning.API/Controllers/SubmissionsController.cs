using ELearning.Application.Submissions.Commands;
using ELearning.Application.Submissions.Dtos;
using ELearning.Application.Submissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubmissionDetailDto>> GetSubmission(Guid id)
    {
        var query = new GetSubmissionDetailQuery { SubmissionId = id };
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
    public async Task<ActionResult<Guid>> CreateSubmission(CreateSubmissionCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            //return CreatedAtAction(nameof(GetSubmission), new { id = result.Value }, result.Value); // Command shouldn't have a result
            return CreatedAtAction(nameof(GetSubmission), result);
        }

        return BadRequest(result.Error);
    }

    [HttpPost("{id}/grade")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GradeSubmission(Guid id, GradeSubmissionCommand command)
    {
        if (id != command.SubmissionId)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.Error.StartsWith("Submission not found")
            ? NotFound(result.Error)
            : BadRequest(result.Error);
    }
}