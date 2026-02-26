using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Services;
using ELearning.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ELearning.Infrastructure.Services;

public class AuthService(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IInstructorRepository instructorRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IUserService userService,
        ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                return AuthResult.Failed("User not found.");
            }

            if (!await userService.VerifyPasswordAsync(user.PasswordHash, password))
            {
                return AuthResult.Failed("Invalid password.");
            }

            if (!user.IsActive)
            {
                return AuthResult.Failed("User account is inactive.");
            }

            user.RecordLogin();
            await userRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var token = await GenerateJwtToken(user);

            return AuthResult.Succeeded(
                token,
                user.Id,
                user.Email.Value,
                user.FullName,
                user.Role.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during authentication for email {Email}", email);
            return AuthResult.Failed("An error occurred during authentication.");
        }
    }

    public async Task<AuthResult> RegisterStudentAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await userService.IsEmailUniqueAsync(email, cancellationToken: cancellationToken))
            {
                return AuthResult.Failed("Email is already in use.");
            }

            var passwordHash = userService.HashPassword(password);

            var student = new Student(
                firstName,
                lastName,
                Email.Create(email),
                passwordHash);

            await studentRepository.AddAsync(student, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var token = await GenerateJwtToken(student);

            return AuthResult.Succeeded(
                token,
                student.Id,
                student.Email.Value,
                student.FullName,
                student.Role.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during student registration for email {Email}", email);
            return AuthResult.Failed("An error occurred during registration.");
        }
    }

    public async Task<AuthResult> RegisterInstructorAsync(string firstName, string lastName, string email, string password, string bio, string expertise, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await userService.IsEmailUniqueAsync(email, cancellationToken: cancellationToken))
            {
                return AuthResult.Failed("Email is already in use.");
            }

            var passwordHash = userService.HashPassword(password);

            var instructor = new Instructor(
                firstName,
                lastName,
                Email.Create(email),
                passwordHash,
                bio,
                expertise);

            await instructorRepository.AddAsync(instructor, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var token = await GenerateJwtToken(instructor);

            return AuthResult.Succeeded(
                token,
                instructor.Id,
                instructor.Email.Value,
                instructor.FullName,
                instructor.Role.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during instructor registration for email {Email}", email);
            return AuthResult.Failed("An error occurred during registration.");
        }
    }

    public Task<string> GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");
        var expiryInDaysValue = jwtSettings["ExpiryInDays"] ?? throw new InvalidOperationException("JwtSettings:ExpiryInDays is not configured.");
        if (!double.TryParse(expiryInDaysValue, out var expiryInDays))
        {
            throw new InvalidOperationException("JwtSettings:ExpiryInDays is invalid.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(expiryInDays);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
