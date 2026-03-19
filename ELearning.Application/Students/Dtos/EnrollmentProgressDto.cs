// <copyright file="EnrollmentProgressDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record EnrollmentProgressDto(
    Guid EnrollmentId,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage,
    int CompletedLessons,
    int TotalLessons,
    int CompletedAssignments,
    int TotalAssignments) : IDto;
