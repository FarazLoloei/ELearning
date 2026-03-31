// <copyright file="Instructor.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate;

using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

public class Instructor : User
{
    /// <summary>
    /// Gets professional biography.
    /// </summary>
    public string Bio { get; private set; } = string.Empty;

    /// <summary>
    /// Gets areas of specialization.
    /// </summary>
    public string Expertise { get; private set; } = string.Empty;

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
}
