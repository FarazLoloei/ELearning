// <copyright file="InstructorCourseDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record InstructorCourseDto(
    Guid Id,
    string Title,
    string Category,
    int EnrollmentsCount,
    string Status,
    DateTime CreatedAt,
    DateTime? PublishedDate) : IDto;
