// <copyright file="GetStudentProgressQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Students.Dtos;
using MediatR;

public sealed record GetStudentProgressQuery : IRequest<Result<StudentProgressDto>>
{
    public Guid StudentId { get; init; }
}
