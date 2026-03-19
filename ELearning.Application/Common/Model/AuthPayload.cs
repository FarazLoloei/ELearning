// <copyright file="AuthPayload.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Model;

public sealed record AuthPayload(
    string Token,
    string? RefreshToken,
    Guid UserId,
    string Email,
    string FullName,
    string Role);
