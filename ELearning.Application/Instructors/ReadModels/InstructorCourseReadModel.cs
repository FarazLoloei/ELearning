// <copyright file="InstructorCourseReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.ReadModels;

public sealed record InstructorCourseReadModel(
    Guid Id,
    string Title,
    int CategoryId,
    int EnrollmentsCount,
    int StatusId,
    DateTime? PublishedDate,
    DateTime CreatedAtUtc);
