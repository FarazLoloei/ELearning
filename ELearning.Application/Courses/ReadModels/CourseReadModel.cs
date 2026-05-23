// <copyright file="CourseReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.ReadModels;

using System.Globalization;

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
    public CourseReadModel(
        string Id,
        string Title,
        string Description,
        string InstructorFirstName,
        string InstructorLastName,
        long CategoryId,
        long LevelId,
        string Price,
        string AverageRating,
        long NumberOfRatings,
        long IsFeatured,
        long DurationHours,
        long DurationMinutes,
        long EnrollmentsCount)
        : this(
            Guid.Parse(Id),
            Title,
            Description,
            InstructorFirstName,
            InstructorLastName,
            checked((int)CategoryId),
            checked((int)LevelId),
            decimal.Parse(Price, NumberStyles.Number, CultureInfo.InvariantCulture),
            decimal.Parse(AverageRating, NumberStyles.Number, CultureInfo.InvariantCulture),
            checked((int)NumberOfRatings),
            IsFeatured != 0,
            checked((int)DurationHours),
            checked((int)DurationMinutes),
            checked((int)EnrollmentsCount))
    {
    }

    public string InstructorName => $"{this.InstructorFirstName} {this.InstructorLastName}".Trim();
}
