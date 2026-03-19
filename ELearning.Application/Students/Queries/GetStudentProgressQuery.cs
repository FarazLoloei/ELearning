// <copyright file="GetStudentProgressQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Students.Dtos;
using MediatR;

public sealed record GetStudentProgressQuery : IRequest<Result<StudentProgressDto>>
{
    public Guid StudentId { get; init; }
}
