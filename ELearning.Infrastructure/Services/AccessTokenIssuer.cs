// <copyright file="AccessTokenIssuer.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ELearning.Application.Auth.Abstractions;
using ELearning.Domain.Entities.UserAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public sealed class AccessTokenIssuer(IConfiguration configuration) : IAccessTokenIssuer
{
    public string IssueToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
