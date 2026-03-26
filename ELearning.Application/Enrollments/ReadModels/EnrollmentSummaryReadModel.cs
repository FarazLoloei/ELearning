// <copyright file="EnrollmentSummaryReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.ReadModels;

public sealed record EnrollmentSummaryReadModel(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage);