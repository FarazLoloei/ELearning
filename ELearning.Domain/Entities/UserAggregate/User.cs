// <copyright file="User.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate;

using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

public class User : BaseEntity, IAggregateRoot<User>
{
    /// <summary>
    /// Gets or sets user's first name.
    /// </summary>
    public string FirstName { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets user's last name.
    /// </summary>
    public string LastName { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets unique identifier for login/communication.
    /// </summary>
    public Email Email { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets secured password storage.
    /// </summary>
    public string PasswordHash { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets link to profile image.
    /// </summary>
    public string? ProfilePictureUrl { get; protected set; }

    /// <summary>
    /// Gets or sets type of user (Student, Instructor, Admin).
    /// </summary>
    public UserRole Role { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether whether account is enabled.
    /// </summary>
    public bool IsActive { get; protected set; }

    /// <summary>
    /// Gets or sets timestamp of most recent login.
    /// </summary>
    public DateTime? LastLoginDate { get; protected set; }

    /// <summary>
    /// Gets or sets optimistic concurrency token.
    /// </summary>
    public byte[]? RowVersion { get; protected set; }

    protected User()
    {
    }

    protected User(string firstName, string lastName, Email email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        }

        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email ?? throw new ArgumentNullException(nameof(email));
        this.PasswordHash = passwordHash;
        this.Role = role;
        this.IsActive = true;
    }

    public string FullName => $"{this.FirstName} {this.LastName}";

    public void UpdatePersonalInfo(string firstName, string lastName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateEmail(Email email)
    {
        this.Email = email ?? throw new ArgumentNullException(nameof(email));
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        }

        this.PasswordHash = passwordHash;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateProfilePicture(string profilePictureUrl)
    {
        this.ProfilePictureUrl = profilePictureUrl;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void SetActive(bool isActive)
    {
        this.IsActive = isActive;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RecordLogin()
    {
        this.LastLoginDate = DateTime.UtcNow;
        this.UpdatedAt(DateTime.UtcNow);
    }
}
