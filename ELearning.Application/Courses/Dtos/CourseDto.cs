// <copyright file="CourseDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Dtos;

using ELearning.Application.Instructors.Dtos;
using ELearning.SharedKernel.Abstractions;

public sealed record CourseDto(
    Guid Id,
    string Title,
    string Description,
    InstructorDto Instructor,
    string Status,
    string Category,
    string Level,
    decimal Price,
    string Duration,
    DateTime? PublishedDate,
    decimal AverageRating,
    int NumberOfRatings,
    IReadOnlyList<ModuleDto> Modules,
    IReadOnlyList<ReviewDto> Reviews) : IDto;
