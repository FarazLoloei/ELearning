using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.API.Models;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = ELearning.API.Models.LoginRequest;

namespace ELearning.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

        if (result.Success)
        {
            return Ok(ApiResponse<AuthResult>.Success(result));
        }

        return UnauthorizedResponse<AuthResult>(result.ErrorMessage ?? "Authentication failed.");
    }

    [HttpPost("register/student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RegisterStudent(RegisterStudentRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterStudentAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            cancellationToken);

        return result.Success
            ? Ok(ApiResponse<AuthResult>.Success(result))
            : BadRequestResponse<AuthResult>(result.ErrorMessage ?? "Registration failed.");
    }

    [HttpPost("register/instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResult>>> RegisterInstructor(RegisterInstructorRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterInstructorAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Bio,
            request.Expertise,
            cancellationToken);

        return result.Success
            ? Ok(ApiResponse<AuthResult>.Success(result))
            : BadRequestResponse<AuthResult>(result.ErrorMessage ?? "Registration failed.");
    }
}
