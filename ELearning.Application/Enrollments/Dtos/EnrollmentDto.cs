using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Enrollments.Dtos;

public sealed record EnrollmentDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage
) : IDto;
