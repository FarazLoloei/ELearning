// <copyright file="GetSubmissionDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;
using MediatR;

/// <summary>
/// Query to get details of a submission.
/// </summary>
public record GetSubmissionDetailQuery : IRequest<Result<SubmissionDetailDto>>
{
    public Guid SubmissionId { get; set; }
}