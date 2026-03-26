// <copyright file="LessonNotFoundException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

using ELearning.Domain.Exceptions;

// Lesson-specific exceptions
public sealed class LessonNotFoundException : DomainException
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Lesson with ID '{lessonId}' was not found.")
    {
    }
}