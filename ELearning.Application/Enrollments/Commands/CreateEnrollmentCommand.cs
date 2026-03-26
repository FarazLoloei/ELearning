// <copyright file="CreateEnrollmentCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record CreateEnrollmentCommand : IRequest<Result>
{
    public Guid CourseId { get; init; }
}
