using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Dtos;

public sealed record StudentDto(
    Guid Id,
    string FullName,
    string Email,
    string ProfilePictureUrl,
    DateTime? LastLoginDate
) : IDto;
