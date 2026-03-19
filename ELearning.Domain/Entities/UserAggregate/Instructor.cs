// <copyright file="Instructor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

public class Instructor : User
{
    private readonly HashSet<Course> courses = new();

    /// <summary>
    /// Gets professional biography.
    /// </summary>
    public string Bio { get; private set; } = string.Empty;

    /// <summary>
    /// Gets areas of specialization.
    /// </summary>
    public string Expertise { get; private set; } = string.Empty;

    /// <summary>
    /// Gets collection of courses created by this instructor.
    /// </summary>
    public IReadOnlyCollection<Course> Courses => this.courses.ToList().AsReadOnly();

    private Instructor()
        : base()
    {
    }

    public Instructor(
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        string bio = "",
        string expertise = "")
        : base(firstName, lastName, email, passwordHash, UserRole.Instructor)
    {
        this.Bio = bio ?? string.Empty;
        this.Expertise = expertise ?? string.Empty;
    }

    public void UpdateBio(string bio)
    {
        if (!string.IsNullOrWhiteSpace(bio))
        {
            this.Bio = bio;
            this.UpdatedAt(DateTime.UtcNow);
        }
    }

    public void UpdateExpertise(string expertise)
    {
        if (!string.IsNullOrWhiteSpace(expertise))
        {
            this.Expertise = expertise;
            this.UpdatedAt(DateTime.UtcNow);
        }
    }

    public bool AddCourse(Course course) => this.courses.Add(course); // HashSet prevents duplicates and returns success status

    public bool RemoveCourse(Course course) => this.courses.Remove(course);
}
