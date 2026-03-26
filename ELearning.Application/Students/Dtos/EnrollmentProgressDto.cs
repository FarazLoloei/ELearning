// <copyright file="EnrollmentProgressDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
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
