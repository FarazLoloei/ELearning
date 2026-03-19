// <copyright file="LessonReadModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.ReadModels;

public sealed record LessonReadModel(
    Guid Id,
    string Title,
    Guid ModuleId,
    int Order);