// <copyright file="InstructorCourseDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
