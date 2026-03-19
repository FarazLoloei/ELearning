// <copyright file="InstructorWithCoursesReadModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Instructors.ReadModels;

public sealed record InstructorWithCoursesReadModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Bio,
    string Expertise,
    string? ProfilePictureUrl,
    decimal AverageRating,
    int TotalStudents,
    int TotalCourses,
    IReadOnlyList<InstructorCourseReadModel> Courses)
{
    public string FullName => $"{this.FirstName} {this.LastName}".Trim();
}
