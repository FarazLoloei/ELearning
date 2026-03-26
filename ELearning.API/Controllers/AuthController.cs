// <copyright file="AuthController.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Controllers;

using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.API.Facades;
using ELearning.API.Models;
using ELearning.Application.Common.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LoginRequest = ELearning.API.Models.LoginRequest;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[EnableRateLimiting("AuthEndpoints")]
public class AuthController(IApiFacade apiFacade) : ApiControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await apiFacade.AuthenticateAsync(request, cancellationToken);

        if (result.Success)
        {
            return this.Ok(ApiResponse<AuthResult>.Success(result));
        }

        return this.UnauthorizedResponse<AuthResult>(result.ErrorMessage ?? "Authentication failed.");
    }

    [HttpPost("register/student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RegisterStudent(RegisterStudentRequest request, CancellationToken cancellationToken)
    {
        var result = await apiFacade.RegisterStudentAsync(request, cancellationToken);

        return result.Success
            ? this.Ok(ApiResponse<AuthResult>.Success(result))
            : this.BadRequestResponse<AuthResult>(result.ErrorMessage ?? "Registration failed.");
    }

    [HttpPost("register/instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RegisterInstructor(RegisterInstructorRequest request, CancellationToken cancellationToken)
    {
        var result = await apiFacade.RegisterInstructorAsync(request, cancellationToken);

        return result.Success
            ? this.Ok(ApiResponse<AuthResult>.Success(result))
            : this.BadRequestResponse<AuthResult>(result.ErrorMessage ?? "Registration failed.");
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await apiFacade.RefreshTokenAsync(request, cancellationToken);

        return result.Success
            ? this.Ok(ApiResponse<AuthResult>.Success(result))
            : this.UnauthorizedResponse<AuthResult>(result.ErrorMessage ?? "Refresh token is invalid.");
    }

    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object?>>> RevokeToken(RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await apiFacade.RevokeTokenAsync(request, cancellationToken);
        return result.IsSuccess ? this.Ok(ApiResponse<object?>.Success(null)) : this.BadRequestResponse<object?>(result.Error);
    }
}
