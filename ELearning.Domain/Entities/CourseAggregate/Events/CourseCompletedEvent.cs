// <copyright file="CourseCompletedEvent.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Events;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel.Abstractions;

public sealed class CourseCompletedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public DateTime OccurredOnUTC { get; }

    public CourseCompletedEvent(Student student, Course course, Enrollment enrollment)
    {
        this.Student = student ?? throw new ArgumentNullException(nameof(student));
        this.Course = course ?? throw new ArgumentNullException(nameof(course));
        this.Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        this.OccurredOnUTC = DateTime.UtcNow;
    }
}