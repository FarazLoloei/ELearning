using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.UserAggregate;

public class User : BaseEntity, IAggregateRoot<User>
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; protected set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; protected set; }

    /// <summary>
    /// Unique identifier for login/communication
    /// </summary>
    public Email Email { get; protected set; }

    /// <summary>
    /// Secured password storage
    /// </summary>
    public string PasswordHash { get; protected set; }

    /// <summary>
    /// Link to profile image
    /// </summary>
    public string ProfilePictureUrl { get; protected set; }

    /// <summary>
    /// Type of user (Student, Instructor, Admin)
    /// </summary>
    public UserRole Role { get; protected set; }

    /// <summary>
    /// Whether account is enabled
    /// </summary>
    public bool IsActive { get; protected set; }

    /// <summary>
    /// Timestamp of most recent login
    /// </summary>
    public DateTime? LastLoginDate { get; protected set; }

    protected User()
    { }

    protected User(string firstName, string lastName, Email email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        FirstName = firstName;
        LastName = lastName;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdatePersonalInfo(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(Email email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfilePicture(string profilePictureUrl)
    {
        ProfilePictureUrl = profilePictureUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}