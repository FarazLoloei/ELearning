// <copyright file="StudentDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record StudentDto(
    Guid Id,
    string FullName,
    string Email,
    string ProfilePictureUrl,
    DateTime? LastLoginDate) : IDto;
