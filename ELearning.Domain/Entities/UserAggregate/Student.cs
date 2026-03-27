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
    private readonly Dictionary<Guid, Enrollment> enrollments = new();

    /// <summary>
    /// Gets courses this student is enrolled in.
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => this.enrollments.Values.ToList().AsReadOnly();

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
        if (course == null)
        {
            throw new ArgumentNullException(nameof(course));
        }

        if (this.enrollments.ContainsKey(course.Id))
        {
            return false; // Already enrolled
        }

        var enrollment = new Enrollment(this.Id, course.Id, null, null);
        this.enrollments[course.Id] = enrollment;

        this.AddDomainEvent(new EnrollmentCreatedEvent(this, course, enrollment));
        return true;
    }

    public bool UnenrollFromCourse(Guid courseId) => this.enrollments.Remove(courseId);
}