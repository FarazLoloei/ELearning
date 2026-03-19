// <copyright file="GetPendingSubmissionsQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;
using ELearning.SharedKernel;
using MediatR;

/// <summary>
/// Query to get pending submissions for an instructor.
/// </summary>
public record GetPendingSubmissionsQuery : IRequest<Result<PaginatedList<SubmissionDto>>>
{
    public Guid InstructorId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}