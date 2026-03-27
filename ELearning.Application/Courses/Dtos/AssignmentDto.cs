// <copyright file="AssignmentDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record AssignmentDto(
    Guid Id,
    string Title,
    string Description,
    string Type,
    int MaxPoints,
    DateTime? DueDate) : IDto;
