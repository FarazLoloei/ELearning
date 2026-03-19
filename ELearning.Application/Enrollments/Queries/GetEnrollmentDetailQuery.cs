// <copyright file="GetEnrollmentDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Dtos;
using MediatR;

public record GetEnrollmentDetailQuery : IRequest<Result<EnrollmentDetailDto>>
{
    public Guid EnrollmentId { get; set; }
}