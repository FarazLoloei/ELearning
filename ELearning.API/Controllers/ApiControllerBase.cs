using ELearning.API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> FromResult<T>(ELearning.Application.Common.Model.Result<T> result, Func<string, bool>? isNotFound = null)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Success(result.Value));
        }

        return BuildFailure<T>(result.Error, isNotFound);
    }

    protected ActionResult<ApiResponse<object?>> FromResult(ELearning.Application.Common.Model.Result result, Func<string, bool>? isNotFound = null)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object?>.Success(null));
        }

        return BuildFailure<object?>(result.Error, isNotFound);
    }

    protected ActionResult<ApiResponse<object?>> CreatedResponse() =>
        StatusCode(StatusCodes.Status201Created, ApiResponse<object?>.Success(null));

    protected ActionResult<ApiResponse<T>> UnauthorizedResponse<T>(string message) =>
        Unauthorized(ApiResponse<T>.Failure("unauthorized", message));

    protected ActionResult<ApiResponse<T>> BadRequestResponse<T>(string message) =>
        BadRequest(ApiResponse<T>.Failure("bad_request", message));

    protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message) =>
        NotFound(ApiResponse<T>.Failure("not_found", message));

    private ActionResult<ApiResponse<T>> BuildFailure<T>(string message, Func<string, bool>? isNotFound)
    {
        if (isNotFound?.Invoke(message) == true)
        {
            return NotFoundResponse<T>(message);
        }

        return BadRequestResponse<T>(message);
    }
}
