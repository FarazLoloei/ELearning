// <copyright file="AuthService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Services;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

public class AuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IUserService userService,
        ILogger<AuthService> logger) : IAuthService
{
    private const int RefreshTokenByteSize = 64;

    public async Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                await this.PersistAuditOnlyAsync(null, "auth.login", false, "User not found", cancellationToken);
                return AuthResult.Failed("Authentication failed.");
            }

            if (!await userService.VerifyPasswordAsync(user.PasswordHash, password))
            {
                await this.PersistAuditOnlyAsync(user.Id, "auth.login", false, "Invalid password", cancellationToken);
                return AuthResult.Failed("Authentication failed.");
            }

            if (!user.IsActive)
            {
                await this.PersistAuditOnlyAsync(user.Id, "auth.login", false, "User inactive", cancellationToken);
                return AuthResult.Failed("Authentication failed.");
            }

            user.RecordLogin();
            await userRepository.UpdateAsync(user, cancellationToken);

            var token = await this.GenerateJwtToken(user);
            var refreshToken = await this.IssueRefreshTokenAsync(user.Id, cancellationToken);
            this.AddSecurityAuditEvent(user.Id, "auth.login", true, "User login succeeded.");

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return AuthResult.Succeeded(
                token,
                refreshToken,
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
                await this.PersistAuditOnlyAsync(null, "auth.register.student", false, "Email already in use", cancellationToken);
                return AuthResult.Failed("Email is already in use.");
            }

            var passwordHash = userService.HashPassword(password);

            var student = new Student(
                firstName,
                lastName,
                Email.Create(email),
                passwordHash);

            await userRepository.AddAsync(student, cancellationToken);

            var token = await this.GenerateJwtToken(student);
            var refreshToken = await this.IssueRefreshTokenAsync(student.Id, cancellationToken);
            this.AddSecurityAuditEvent(student.Id, "auth.register.student", true, "Student registration succeeded.");

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return AuthResult.Succeeded(
                token,
                refreshToken,
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
                await this.PersistAuditOnlyAsync(null, "auth.register.instructor", false, "Email already in use", cancellationToken);
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

            await userRepository.AddAsync(instructor, cancellationToken);

            var token = await this.GenerateJwtToken(instructor);
            var refreshToken = await this.IssueRefreshTokenAsync(instructor.Id, cancellationToken);
            this.AddSecurityAuditEvent(instructor.Id, "auth.register.instructor", true, "Instructor registration succeeded.");

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return AuthResult.Succeeded(
                token,
                refreshToken,
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

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshTokenHash = HashToken(refreshToken);
            var existingToken = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash, cancellationToken);

            if (existingToken == null || existingToken.RevokedAtUtc.HasValue || existingToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                await this.PersistAuditOnlyAsync(null, "auth.refresh", false, "Refresh token invalid or expired", cancellationToken);
                return AuthResult.Failed("Refresh token is invalid.");
            }

            var user = await userRepository.GetByIdForUpdateAsync(existingToken.UserId, cancellationToken);
            if (user == null || !user.IsActive)
            {
                await this.PersistAuditOnlyAsync(existingToken.UserId, "auth.refresh", false, "User not found or inactive", cancellationToken);
                return AuthResult.Failed("Refresh token is invalid.");
            }

            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenHash = HashToken(newRefreshToken);

            existingToken.RevokedAtUtc = DateTime.UtcNow;
            existingToken.RevokedReason = "Rotated";
            existingToken.ReplacedByTokenHash = newRefreshTokenHash;

            dbContext.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(this.GetRefreshTokenExpiryDays()),
                CreatedByIp = this.GetRemoteIpAddress(),
                UserAgent = this.GetUserAgent(),
            });

            var jwtToken = await this.GenerateJwtToken(user);
            this.AddSecurityAuditEvent(user.Id, "auth.refresh", true, "Refresh token rotated.");
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return AuthResult.Succeeded(
                jwtToken,
                newRefreshToken,
                user.Id,
                user.Email.Value,
                user.FullName,
                user.Role.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token refresh.");
            return AuthResult.Failed("An error occurred while refreshing token.");
        }
    }

    public async Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        try
        {
            var refreshTokenHash = HashToken(refreshToken);
            var existingToken = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash, cancellationToken);

            if (existingToken == null)
            {
                await this.PersistAuditOnlyAsync(null, "auth.revoke", false, "Refresh token not found", cancellationToken);
                return Result.Failure("Refresh token was not found.");
            }

            var currentUserId = currentUserService.UserId.Value;
            if (existingToken.UserId != currentUserId && !currentUserService.IsInRole("Admin"))
            {
                throw new ForbiddenAccessException();
            }

            if (!existingToken.RevokedAtUtc.HasValue)
            {
                existingToken.RevokedAtUtc = DateTime.UtcNow;
                existingToken.RevokedReason = "Revoked by request";
            }

            this.AddSecurityAuditEvent(existingToken.UserId, "auth.revoke", true, "Refresh token revoked.");
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ForbiddenAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token revocation.");
            return Result.Failure("An error occurred while revoking token.");
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
        var expiry = DateTime.UtcNow.AddDays(expiryInDays);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.Name),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    private async Task<string> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var rawRefreshToken = GenerateRefreshToken();
        var tokenHash = HashToken(rawRefreshToken);

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(this.GetRefreshTokenExpiryDays()),
            CreatedByIp = this.GetRemoteIpAddress(),
            UserAgent = this.GetUserAgent(),
        });

        await Task.CompletedTask;
        return rawRefreshToken;
    }

    private int GetRefreshTokenExpiryDays()
    {
        var value = configuration["JwtSettings:RefreshTokenExpiryInDays"];
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : 14;
    }

    private static string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[RefreshTokenByteSize];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }

    private void AddSecurityAuditEvent(Guid? userId, string eventType, bool succeeded, string detail)
    {
        dbContext.SecurityAuditEvents.Add(new SecurityAuditEvent
        {
            UserId = userId,
            EventType = eventType,
            Succeeded = succeeded,
            Detail = detail,
            IpAddress = this.GetRemoteIpAddress(),
            UserAgent = this.GetUserAgent(),
            OccurredOnUtc = DateTime.UtcNow,
        });
    }

    private async Task PersistAuditOnlyAsync(Guid? userId, string eventType, bool succeeded, string detail, CancellationToken cancellationToken)
    {
        try
        {
            this.AddSecurityAuditEvent(userId, eventType, succeeded, detail);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to persist audit event {EventType}", eventType);
        }
    }

    private string? GetRemoteIpAddress() => httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    private string? GetUserAgent() => httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.ToString();
}
