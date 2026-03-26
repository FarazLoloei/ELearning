// <copyright file="EnrollmentDetailDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Dtos;

using ELearning.Application.Submissions.Dtos;
using ELearning.SharedKernel.Abstractions;

public sealed record EnrollmentDetailDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage,
    IReadOnlyList<LessonProgressDto> LessonProgress,
    IReadOnlyList<SubmissionDto> Submissions,
    decimal? CourseRating,
    string? Review) : IDto;
