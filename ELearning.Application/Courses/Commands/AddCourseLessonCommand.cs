// <copyright file="AddCourseLessonCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record AddCourseLessonCommand(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    string Content,
    int TypeId,
    int Order,
    int DurationHours,
    int DurationMinutes,
    string? VideoUrl) : IRequest<Result<Guid>>;
