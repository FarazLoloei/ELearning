// <copyright file="GetCoursesListQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using MediatR;

public record GetCoursesListQuery : IRequest<Result<PaginatedList<CourseListDto>>>, IPaginatable
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchTerm { get; set; }

    public int? CategoryId { get; set; }

    public int? LevelId { get; set; }

    public bool? IsFeatured { get; set; }
}