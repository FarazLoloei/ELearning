// <copyright file="JwtBearerOptionsSetup.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using System.Text;
using ELearning.Application.Common.Model;
using ELearning.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public sealed class JwtBearerOptionsSetup(IOptions<JwtSettingsOptions> jwtSettingsOptions) :
    IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (!string.Equals(name, JwtBearerDefaults.AuthenticationScheme, StringComparison.Ordinal))
        {
            return;
        }

        var jwtSettings = jwtSettingsOptions.Value;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                if (context.Response.HasStarted)
                {
                    return;
                }

                var problemDetails = ApiProblemDetailsFactory.Create(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Authentication is required to access this resource.",
                    ApplicationErrorCodes.AuthenticationUnauthorized);

                await ApiProblemDetailsFactory.WriteAsync(context.HttpContext, problemDetails);
            },
            OnForbidden = async context =>
            {
                if (context.Response.HasStarted)
                {
                    return;
                }

                var problemDetails = ApiProblemDetailsFactory.Create(
                    context.HttpContext,
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    "You do not have permission to access this resource.",
                    ApplicationErrorCodes.AuthorizationForbidden);

                await ApiProblemDetailsFactory.WriteAsync(context.HttpContext, problemDetails);
            },
        };
    }

    public void Configure(JwtBearerOptions options) =>
        this.Configure(JwtBearerDefaults.AuthenticationScheme, options);
}
