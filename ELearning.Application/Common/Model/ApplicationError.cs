// <copyright file="ApplicationError.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Model;

public sealed record ApplicationError(string Code, string Message, ErrorType Type)
{
    public static ApplicationError BadRequest(
        string message,
        string code = ApplicationErrorCodes.RequestInvalid) =>
        new(code, message, ErrorType.BadRequest);

    public static ApplicationError Validation(
        string message,
        string code = ApplicationErrorCodes.ValidationFailed) =>
        new(code, message, ErrorType.Validation);

    public static ApplicationError NotFound(
        string message,
        string code = ApplicationErrorCodes.ResourceNotFound) =>
        new(code, message, ErrorType.NotFound);

    public static ApplicationError Conflict(
        string message,
        string code = ApplicationErrorCodes.BusinessRuleConflict) =>
        new(code, message, ErrorType.Conflict);

    public static ApplicationError Unauthorized(
        string message,
        string code = ApplicationErrorCodes.AuthenticationUnauthorized) =>
        new(code, message, ErrorType.Unauthorized);

    public static ApplicationError Forbidden(
        string message,
        string code = ApplicationErrorCodes.AuthorizationForbidden) =>
        new(code, message, ErrorType.Forbidden);
}

public enum ErrorType
{
    BadRequest = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4,
    Unauthorized = 5,
    Forbidden = 6,
}

public static class ApplicationErrorCodes
{
    public const string ValidationFailed = "validation.failed";
    public const string RequestInvalid = "request.invalid";
    public const string RouteIdMismatch = "request.route_id_mismatch";
    public const string ResourceNotFound = "resource.not_found";
    public const string BusinessRuleConflict = "conflict.business_rule";
    public const string ConcurrencyConflict = "conflict.concurrency";
    public const string AuthenticationUnauthorized = "authentication.unauthorized";
    public const string AuthorizationForbidden = "authorization.forbidden";
    public const string UnexpectedError = "error.unexpected";
}
