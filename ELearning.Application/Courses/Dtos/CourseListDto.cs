// <copyright file="CourseListDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record CourseListDto(
    Guid Id,
    string Title,
    string Description,
    string InstructorName,
    string Category,
    string Level,
    decimal Price,
    decimal AverageRating,
    int NumberOfRatings,
    bool IsFeatured,
    string Duration,
    int EnrollmentsCount) : IDto;
