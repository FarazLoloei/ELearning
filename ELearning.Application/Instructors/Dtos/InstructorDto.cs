// <copyright file="InstructorDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record InstructorDto(
    Guid Id,
    string FullName,
    string Email,
    string Bio,
    string Expertise,
    string ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses) : IDto;
