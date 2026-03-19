// <copyright file="GetCourseDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using MediatR;

public record GetCourseDetailQuery : IRequest<Result<CourseDto>>
{
    public Guid CourseId { get; set; }
}
