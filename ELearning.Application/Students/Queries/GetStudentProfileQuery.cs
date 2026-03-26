// <copyright file="GetStudentProfileQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Students.Dtos;
using MediatR;

public sealed record GetStudentProfileQuery : IRequest<Result<StudentDto>>
{
    public Guid StudentId { get; init; }
}
