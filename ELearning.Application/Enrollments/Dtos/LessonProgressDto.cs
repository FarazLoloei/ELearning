// <copyright file="LessonProgressDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Dtos;

using ELearning.SharedKernel.Abstractions;

// DTO for lesson progress
public sealed record LessonProgressDto(
    Guid LessonId,
    string LessonTitle,
    string Status,
    DateTime? CompletedDate,
    int TimeSpentSeconds) : IDto;
