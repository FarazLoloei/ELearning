// <copyright file="AuthResult.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Model;

public sealed record AuthResult
{
    public bool Success { get; }

    public AuthPayload? Data { get; }

    public string? ErrorMessage { get; }

    public string? Token => this.Data?.Token;

    public string? RefreshToken => this.Data?.RefreshToken;

    public Guid? UserId => this.Data?.UserId;

    public string? Email => this.Data?.Email;

    public string? FullName => this.Data?.FullName;

    public string? Role => this.Data?.Role;

    private AuthResult(bool success, AuthPayload? data, string? errorMessage)
    {
        var isValidSuccess = success && data is not null && string.IsNullOrWhiteSpace(errorMessage);
        var isValidFailure = !success && data is null && !string.IsNullOrWhiteSpace(errorMessage);

        if (!isValidSuccess && !isValidFailure)
        {
            throw new InvalidOperationException("AuthResult must be either a success with data or a failure with error.");
        }

        this.Success = success;
        this.Data = data;
        this.ErrorMessage = errorMessage;
    }

    public static AuthResult Succeeded(AuthPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return new AuthResult(true, payload, null);
    }

    public static AuthResult Succeeded(
        string token,
        string? refreshToken,
        Guid userId,
        string email,
        string fullName,
        string role)
        => Succeeded(new AuthPayload(token, refreshToken, userId, email, fullName, role));

    public static AuthResult Failed(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        return new AuthResult(false, null, errorMessage);
    }
}
