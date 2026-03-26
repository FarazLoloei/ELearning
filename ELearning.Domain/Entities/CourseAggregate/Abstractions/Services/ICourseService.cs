// <copyright file="ICourseService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;

using ELearning.Domain.ValueObjects;

public interface ICourseService
{
    Task<bool> CanPublishCourseAsync(Course course);

    Task UpdateCourseRatingAsync(Course course, Rating newRating, Rating? oldRating = null);

    Task<bool> IsCourseTitleUniqueForInstructorAsync(string title, Guid instructorId, Guid? excludeCourseId = null);

    Task<decimal> CalculateAverageCompletionTimeAsync(Guid courseId);
}