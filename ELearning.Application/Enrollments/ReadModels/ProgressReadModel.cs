// <copyright file="ProgressReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.ReadModels;

public sealed record ProgressReadModel(
    Guid Id,
    Guid EnrollmentId,
    Guid LessonId,
    int StatusId,
    string StatusName,
    DateTime? CompletedDate,
    int TimeSpentSeconds);