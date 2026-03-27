// <copyright file="CourseNotFoundException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class CourseNotFoundException : DomainException
{
    public CourseNotFoundException(Guid courseId)
        : base($"Course with ID '{courseId}' was not found.")
    {
    }
}