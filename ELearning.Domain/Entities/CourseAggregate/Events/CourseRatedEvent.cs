// <copyright file="CourseRatedEvent.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Events;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel.Abstractions;

public sealed class CourseRatedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public Rating Rating { get; }

    public DateTime OccurredOnUTC { get; }

    public CourseRatedEvent(Student student, Course course, Enrollment enrollment, Rating rating)
    {
        this.Student = student ?? throw new ArgumentNullException(nameof(student));
        this.Course = course ?? throw new ArgumentNullException(nameof(course));
        this.Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        this.Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        this.OccurredOnUTC = DateTime.UtcNow;
    }
}