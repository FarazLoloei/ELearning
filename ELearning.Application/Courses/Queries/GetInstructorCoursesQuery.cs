// <copyright file="GetInstructorCoursesQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Instructors.Dtos;
using MediatR;

/// <summary>
/// Query to get instructor with their courses.
/// </summary>
public record GetInstructorCoursesQuery : IRequest<Result<InstructorCoursesDto>>
{
    public Guid InstructorId { get; set; }
}