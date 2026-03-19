// <copyright file="StudentProgressDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record StudentProgressDto(
    Guid StudentId,
    string StudentName,
    int CompletedCourses,
    int InProgressCourses,
    IReadOnlyList<EnrollmentProgressDto> Enrollments) : IDto;
