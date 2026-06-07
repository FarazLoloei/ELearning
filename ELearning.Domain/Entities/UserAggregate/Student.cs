// <copyright file="Student.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

public class Student : User
{
    private readonly List<Enrollment> enrollments = [];

    /// <summary>
    /// Gets courses this student is enrolled in.
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => this.enrollments.AsReadOnly();

    private Student()
        : base()
    {
    }

    public Student(string firstName, string lastName, Email email, string passwordHash)
        : base(firstName, lastName, email, passwordHash, UserRole.Student)
    {
    }

    public bool EnrollInCourse(Course course)
    {
        ArgumentNullException.ThrowIfNull(course);

        if (this.enrollments.Any(enrollment => enrollment.CourseId == course.Id))
        {
            return false;
        }

        var enrollment = new Enrollment(this.Id, course.Id, null, null);
        this.enrollments.Add(enrollment);

        this.AddDomainEvent(new EnrollmentCreatedEvent(this, course, enrollment));
        return true;
    }

    public bool UnenrollFromCourse(Guid courseId)
    {
        var enrollment = this.enrollments.FirstOrDefault(existingEnrollment => existingEnrollment.CourseId == courseId);
        if (enrollment is null)
        {
            return false;
        }

        return this.enrollments.Remove(enrollment);
    }
}
