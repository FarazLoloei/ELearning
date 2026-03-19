// <copyright file="LessonDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
