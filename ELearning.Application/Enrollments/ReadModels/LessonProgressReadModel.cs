// <copyright file="LessonProgressReadModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.ReadModels;

public sealed record LessonProgressReadModel(
    Guid LessonId,
    string LessonTitle,
    string Status,
    DateTime? CompletedDate,
    int TimeSpentSeconds);