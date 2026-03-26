// <copyright file="GradeSubmissionCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record GradeSubmissionCommand : IRequest<Result>
{
    public Guid SubmissionId { get; init; }

    public int Score { get; init; }

    public string Feedback { get; init; } = string.Empty;
}
