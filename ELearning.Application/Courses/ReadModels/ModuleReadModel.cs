// <copyright file="ModuleReadModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Abstractions;

public sealed record ModuleReadModel(
    Guid Id,
    string Title,
    string Description,
    int Order,
    Guid CourseId);