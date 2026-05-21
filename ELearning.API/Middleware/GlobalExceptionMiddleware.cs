// <copyright file="GlobalExceptionMiddleware.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Middleware;

using ELearning.API.Infrastructure;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await this.HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, code, detail) = exception switch
        {
            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                ApplicationErrorCodes.ValidationFailed,
                exception.Message),
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                ApplicationErrorCodes.ResourceNotFound,
                exception.Message),
            ForbiddenAccessException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                ApplicationErrorCodes.AuthorizationForbidden,
                exception.Message),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ApplicationErrorCodes.AuthenticationUnauthorized,
                exception.Message),
            DomainApplicationException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                ApplicationErrorCodes.RequestInvalid,
                exception.Message),
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Concurrency conflict",
                ApplicationErrorCodes.ConcurrencyConflict,
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                ApplicationErrorCodes.UnexpectedError,
                "An unexpected error occurred."),
        };

        if (statusCode >= 500)
        {
            logger.LogError(exception, "Unhandled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        ProblemDetails problemDetails = exception is ValidationException validationException
            ? ApiProblemDetailsFactory.CreateValidation(context, validationException.Errors, detail)
            : ApiProblemDetailsFactory.Create(context, statusCode, title, detail, code);

        await ApiProblemDetailsFactory.WriteAsync(context, problemDetails);
    }
}
