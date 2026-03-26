// <copyright file="LessonDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record LessonDto(
    Guid Id,
    string Title,
    string Content,
    string Type,
    string VideoUrl,
    string Duration,
    int Order) : IDto;
