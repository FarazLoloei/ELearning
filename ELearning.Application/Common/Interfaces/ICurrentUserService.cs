// <copyright file="ICurrentUserService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? UserEmail { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}
