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
        IConfiguration configuration,
        IUserService userService,
        ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                return new AuthResult { Success = false, ErrorMessage = "User not found." };
            }

            if (!await userService.VerifyPasswordAsync(user.PasswordHash, password))
            {
                return new AuthResult { Success = false, ErrorMessage = "Invalid password." };
            }

            if (!user.IsActive)
            {
                return new AuthResult { Success = false, ErrorMessage = "User account is inactive." };
            }

            // Record login
            user.RecordLogin();
            await userRepository.UpdateAsync(user);

            var token = await GenerateJwtToken(user);

            return new AuthResult
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Role = user.Role.Name
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during authentication for email {Email}", email);
            return new AuthResult { Success = false, ErrorMessage = "An error occurred during authentication." };
        }
    }

    public async Task<AuthResult> RegisterStudentAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            // Check if email is already in use
            if (!await userService.IsEmailUniqueAsync(email))
            {
                return new AuthResult { Success = false, ErrorMessage = "Email is already in use." };
            }

            // Hash password
            var passwordHash = await userService.HashPasswordAsync(password);

            // Create student entity
            var student = new Student(
                firstName,
                lastName,
                Email.Create(email),
                passwordHash);

            // Save student
            await studentRepository.AddAsync(student);

            // Generate token
            var token = await GenerateJwtToken(student);

            return new AuthResult
            {
                Success = true,
                Token = token,
                UserId = student.Id,
                Email = student.Email.Value,
                FullName = student.FullName,
                Role = student.Role.Name
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during student registration for email {Email}", email);
            return new AuthResult { Success = false, ErrorMessage = "An error occurred during registration." };
        }
    }

    public async Task<AuthResult> RegisterInstructorAsync(string firstName, string lastName, string email, string password, string bio, string expertise)
    {
        try
        {
            // Check if email is already in use
            if (!await userService.IsEmailUniqueAsync(email))
            {
                return new AuthResult { Success = false, ErrorMessage = "Email is already in use." };
            }

            // Hash password
            var passwordHash = await userService.HashPasswordAsync(password);

            // Create instructor entity
            var instructor = new Instructor(
                firstName,
                lastName,
                Email.Create(email),
                passwordHash,
                bio,
                expertise);

            // Save instructor
            await instructorRepository.AddAsync(instructor);

            // Generate token
            var token = await GenerateJwtToken(instructor);

            return new AuthResult
            {
                Success = true,
                Token = token,
                UserId = instructor.Id,
                Email = instructor.Email.Value,
                FullName = instructor.FullName,
                Role = instructor.Role.Name
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during instructor registration for email {Email}", email);
            return new AuthResult { Success = false, ErrorMessage = "An error occurred during registration." };
        }
    }

    public async Task<string> GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(double.Parse(jwtSettings["ExpiryInDays"]));

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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}