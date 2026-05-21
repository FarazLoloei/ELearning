// <copyright file="AddCourseModuleCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record AddCourseModuleCommand(
    Guid CourseId,
    string Title,
    string Description,
    int Order) : IRequest<Result<Guid>>;
