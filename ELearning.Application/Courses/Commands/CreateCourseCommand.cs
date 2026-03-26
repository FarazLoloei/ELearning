// <copyright file="CreateCourseCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

/// <summary>
/// Command to create a new course.
/// </summary>
public record CreateCourseCommand(
    string Title,
    string Description,
    int CategoryId,
    int LevelId,
    decimal Price,
    int DurationHours,
    int DurationMinutes) : IRequest<Result>;