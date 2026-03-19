// <copyright file="ReviewDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record ReviewDto(
    Guid Id,
    string StudentName,
    decimal Rating,
    string Comment,
    DateTime CreatedAt) : IDto;
