// <copyright file="AssignmentDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
