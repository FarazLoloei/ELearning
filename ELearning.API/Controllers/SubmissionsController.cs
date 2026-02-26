using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.Application.Submissions.Commands;
using ELearning.Application.Submissions.Dtos;
using ELearning.Application.Submissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class SubmissionsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubmissionDetailDto>>> GetSubmission(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSubmissionDetailQuery { SubmissionId = id };
        var result = await mediator.Send(query, cancellationToken);
        return FromResult(result, error => error.StartsWith("Submission not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object?>>> CreateSubmission(
        CreateSubmissionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequestResponse<object?>(result.Error);
        }

        return CreatedResponse();
    }

    [HttpPost("{id:guid}/grade")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> GradeSubmission(
        Guid id,
        GradeSubmissionCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.SubmissionId)
        {
            return BadRequestResponse<object?>("Route id does not match payload SubmissionId.");
        }

        var result = await mediator.Send(command, cancellationToken);
        return FromResult(result, error => error.StartsWith("Submission not found", StringComparison.OrdinalIgnoreCase));
    }
}
