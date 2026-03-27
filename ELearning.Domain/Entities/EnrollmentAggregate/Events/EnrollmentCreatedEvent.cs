// <copyright file="EnrollmentCreatedEvent.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Events;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel.Abstractions;

public sealed class EnrollmentCreatedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public DateTime OccurredOnUTC { get; }

    public EnrollmentCreatedEvent(Student student, Course course, Enrollment enrollment)
    {
        this.Student = student ?? throw new ArgumentNullException(nameof(student));
        this.Course = course ?? throw new ArgumentNullException(nameof(course));
        this.Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        this.OccurredOnUTC = DateTime.UtcNow;
    }
}