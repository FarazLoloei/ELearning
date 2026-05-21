// <copyright file="ApiControllerBase.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Controllers;

using ELearning.API.Contracts;
using ELearning.API.Infrastructure;
using ELearning.Application.Common.Model;
using Microsoft.AspNetCore.Mvc;
using ApplicationModel = ELearning.Application.Common.Model;
using Result = ELearning.Application.Common.Model.Result;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> FromResult<T>(ApplicationModel.Result<T> result)
    {
        if (result.IsSuccess)
        {
            return this.Ok(ApiResponse<T>.Success(result.Value));
        }

        return this.FromError<T>(result.ErrorDetails);
    }

    protected ActionResult<ApiResponse<object?>> FromResult(Result result)
    {
        if (result.IsSuccess)
        {
            return this.Ok(ApiResponse<object?>.Success(null));
        }

        return this.FromError<object?>(result.ErrorDetails);
    }

    protected ActionResult<ApiResponse<object?>> CreatedResponse() =>
        this.StatusCode(StatusCodes.Status201Created, ApiResponse<object?>.Success(null));

    protected ActionResult<ApiResponse<T>> UnauthorizedResponse<T>(string message) =>
        this.FromError<T>(ApplicationError.Unauthorized(message));

    protected ActionResult<ApiResponse<T>> BadRequestResponse<T>(string message) =>
        this.FromError<T>(ApplicationError.BadRequest(message));

    protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message) =>
        this.FromError<T>(ApplicationError.NotFound(message));

    protected ActionResult<ApiResponse<T>> RouteIdMismatchResponse<T>(string payloadIdName) =>
        this.FromError<T>(ApplicationError.BadRequest(
            $"Route id does not match payload {payloadIdName}.",
            ApplicationErrorCodes.RouteIdMismatch));

    protected ActionResult<ApiResponse<T>> FromError<T>(ApplicationError? error)
    {
        var problemDetails = ApiProblemDetailsFactory.Create(
            this.HttpContext,
            error ?? ApplicationError.BadRequest("The request could not be completed."));

        return ApiProblemDetailsFactory.ToObjectResult(problemDetails);
    }
}
