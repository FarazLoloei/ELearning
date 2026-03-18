namespace ELearning.Application.Students.ReadModels;

public sealed record StudentReadModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? ProfilePictureUrl,
    DateTime? LastLoginDate)
{
    public string FullName => $"{FirstName} {LastName}".Trim();
}
