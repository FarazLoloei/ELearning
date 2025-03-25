using ELearning.API.Models;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = ELearning.API.Models.LoginRequest;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
    {
        var result = await authService.AuthenticateAsync(request.Email, request.Password);

        if (result.Success)
        {
            return Ok(result);
        }

        return Unauthorized(result);
    }

    [HttpPost("register/student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResult>> RegisterStudent(RegisterStudentRequest request)
    {
        var result = await authService.RegisterStudentAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("register/instructor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResult>> RegisterInstructor(RegisterInstructorRequest request)
    {
        var result = await authService.RegisterInstructorAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Bio,
            request.Expertise);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}