// <copyright file="ModuleDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record ModuleDto(
    Guid Id,
    string Title,
    string Description,
    int Order,
    IReadOnlyList<LessonDto> Lessons,
    IReadOnlyList<AssignmentDto> Assignments) : IDto;
