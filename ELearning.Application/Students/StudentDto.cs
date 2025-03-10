namespace ELearning.Application.Students;

public readonly record struct StudentDto(
    Guid Id,
    string FullName,
    string Email,
    string ProfilePictureUrl,
    DateTime? LastLoginDate
);