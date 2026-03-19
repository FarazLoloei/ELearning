// <copyright file="StudentDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record StudentDto(
    Guid Id,
    string FullName,
    string Email,
    string ProfilePictureUrl,
    DateTime? LastLoginDate) : IDto;
