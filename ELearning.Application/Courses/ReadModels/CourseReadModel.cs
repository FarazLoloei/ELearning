// <copyright file="CourseReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.ReadModels;

public sealed record CourseReadModel(
    Guid Id,
    string Title,
    string Description,
    string InstructorFirstName,
    string InstructorLastName,
    int CategoryId,
    int LevelId,
    decimal Price,
    decimal AverageRating,
    int NumberOfRatings,
    bool IsFeatured,
    int DurationHours,
    int DurationMinutes,
    int EnrollmentsCount)
{
    public string InstructorName => $"{this.InstructorFirstName} {this.InstructorLastName}".Trim();
}