// <copyright file="UpdateEnrollmentStatusCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands;

using ELearning.Application.Common.Model;
using MediatR;

/// <summary>
/// Command to update the status of an enrollment.
/// </summary>
public sealed record UpdateEnrollmentStatusCommand : IRequest<Result>
{
    public Guid EnrollmentId { get; init; }

    public string Status { get; init; } = string.Empty;
}
