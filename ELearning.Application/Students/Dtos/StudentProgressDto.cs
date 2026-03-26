// <copyright file="StudentProgressDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record StudentProgressDto(
    Guid StudentId,
    string StudentName,
    int CompletedCourses,
    int InProgressCourses,
    IReadOnlyList<EnrollmentProgressDto> Enrollments) : IDto;
