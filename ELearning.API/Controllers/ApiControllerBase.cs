// <copyright file="ApiControllerBase.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Controllers;

using ELearning.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using ApplicationModel = ELearning.Application.Common.Model;
using Result = ELearning.Application.Common.Model.Result;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> FromResult<T>(ApplicationModel.Result<T> result, Func<string, bool>? isNotFound = null)
    {
        if (result.IsSuccess)
        {
            return this.Ok(ApiResponse<T>.Success(result.Value));
        }

        return this.BuildFailure<T>(result.Error, isNotFound);
    }

    protected ActionResult<ApiResponse<object?>> FromResult(Result result, Func<string, bool>? isNotFound = null)
    {
        if (result.IsSuccess)
        {
            return this.Ok(ApiResponse<object?>.Success(null));
        }

        return this.BuildFailure<object?>(result.Error, isNotFound);
    }

    protected ActionResult<ApiResponse<object?>> CreatedResponse() =>
        this.StatusCode(StatusCodes.Status201Created, ApiResponse<object?>.Success(null));

    protected ActionResult<ApiResponse<T>> UnauthorizedResponse<T>(string message) =>
        this.Unauthorized(ApiResponse<T>.Failure("unauthorized", message));

    protected ActionResult<ApiResponse<T>> BadRequestResponse<T>(string message) =>
        this.BadRequest(ApiResponse<T>.Failure("bad_request", message));

    protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message) =>
        this.NotFound(ApiResponse<T>.Failure("not_found", message));

    private ActionResult<ApiResponse<T>> BuildFailure<T>(string message, Func<string, bool>? isNotFound)
    {
        if (isNotFound?.Invoke(message) == true)
        {
            return this.NotFoundResponse<T>(message);
        }

        return this.BadRequestResponse<T>(message);
    }
}