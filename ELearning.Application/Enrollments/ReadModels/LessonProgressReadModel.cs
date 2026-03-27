// <copyright file="LessonProgressReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.ReadModels;

public sealed record LessonProgressReadModel(
    Guid LessonId,
    string LessonTitle,
    string Status,
    DateTime? CompletedDate,
    int TimeSpentSeconds);