// <copyright file="GetCourseDetailQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using MediatR;

public record GetCourseDetailQuery : IRequest<Result<CourseDto>>
{
    public Guid CourseId { get; set; }
}
