// <copyright file="StudentCourseReadModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.ReadModels;

public sealed record StudentCourseReadModel(
    Guid Id,
    string Title,
    int CategoryId,
    int StatusId,
    DateTime PublishedDate);