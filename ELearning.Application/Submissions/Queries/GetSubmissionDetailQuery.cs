// <copyright file="GetSubmissionDetailQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
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