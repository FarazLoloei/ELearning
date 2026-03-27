// <copyright file="CourseReviewReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.ReadModels;

public sealed record CourseReviewReadModel(
    Guid Id,
    string StudentName,
    decimal Rating,
    string Comment,
    DateTime CreatedAt);
