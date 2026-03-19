// <copyright file="CreateEnrollmentCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record CreateEnrollmentCommand : IRequest<Result>
{
    public Guid CourseId { get; init; }
}
