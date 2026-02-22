using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Dtos;

public readonly record struct StudentDto(
    Guid Id,
    string FullName,
    string Email,
    string ProfilePictureUrl,
    DateTime? LastLoginDate
) : IDto;