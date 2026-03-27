// <copyright file="CreateSubmissionCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record CreateSubmissionCommand : IRequest<Result>
{
    public Guid AssignmentId { get; init; }

    /// <summary>
    /// Gets text content of the submission.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets uRL to any uploaded file for the submission.
    /// </summary>
    public string FileUrl { get; init; } = string.Empty;
}
