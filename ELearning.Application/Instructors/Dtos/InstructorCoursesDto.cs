// <copyright file="InstructorCoursesDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record InstructorCoursesDto(
    Guid Id,
    string FullName,
    string Email,
    string Bio,
    string Expertise,
    string ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses,
    IReadOnlyList<InstructorCourseDto> Courses) : IDto;
