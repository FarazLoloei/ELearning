// <copyright file="GetStudentEnrollmentsQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using MediatR;

public sealed record GetStudentEnrollmentsQuery : IRequest<Result<PaginatedList<EnrollmentDto>>>, IPaginatable
{
    public Guid StudentId { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
