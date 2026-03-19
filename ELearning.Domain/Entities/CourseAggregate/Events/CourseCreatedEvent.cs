// <copyright file="CourseCreatedEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Events;

using ELearning.SharedKernel.Abstractions;

public sealed class CourseCreatedEvent : IDomainEvent
{
    public Course Course { get; }

    public DateTime OccurredOnUTC { get; }

    public CourseCreatedEvent(Course course)
    {
        this.Course = course ?? throw new ArgumentNullException(nameof(course));
        this.OccurredOnUTC = DateTime.UtcNow;
    }
}