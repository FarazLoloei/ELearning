// <copyright file="ModuleReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Abstractions;

public sealed record ModuleReadModel(
    Guid Id,
    string Title,
    string Description,
    int Order,
    Guid CourseId);