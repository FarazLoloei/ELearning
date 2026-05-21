// <copyright file="ApiProblemDetailsFactory.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using ELearning.Application.Common.Model;
using Microsoft.AspNetCore.Mvc;

public static class ApiProblemDetailsFactory
{
    public const string ContentType = "application/problem+json";

    public static ProblemDetails Create(HttpContext httpContext, ApplicationError error)
    {
        var statusCode = GetStatusCode(error.Type);
        return Create(httpContext, statusCode, GetTitle(error.Type), error.Message, error.Code);
    }

    public static ProblemDetails Create(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string code)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        AddExtensions(problemDetails, httpContext, code);
        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidation(
        HttpContext httpContext,
        IDictionary<string, string[]> errors,
        string detail = "One or more validation failures have occurred.")
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        AddExtensions(problemDetails, httpContext, ApplicationErrorCodes.ValidationFailed);
        return problemDetails;
    }

    public static ObjectResult ToObjectResult(ProblemDetails problemDetails)
    {
        var result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
        };

        result.ContentTypes.Add(ContentType);
        return result;
    }

    public static async Task WriteAsync(HttpContext httpContext, ProblemDetails problemDetails)
    {
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = ContentType;

        if (problemDetails is ValidationProblemDetails validationProblemDetails)
        {
            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, options: null, contentType: ContentType);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, options: null, contentType: ContentType);
    }

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest,
        };

    private static string GetTitle(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "Validation failed",
            ErrorType.BadRequest => "Invalid request",
            ErrorType.NotFound => "Resource not found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            _ => "Invalid request",
        };

    private static void AddExtensions(ProblemDetails problemDetails, HttpContext httpContext, string code)
    {
        problemDetails.Extensions["code"] = code;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
    }
}
