// <copyright file="AddCourseAssignmentCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record AddCourseAssignmentCommand(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    string Description,
    int TypeId,
    int MaxPoints,
    DateTime? DueDate) : IRequest<Result<Guid>>;
