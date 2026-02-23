using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Dtos;

public sealed record StudentProgressDto(
    Guid StudentId,
    string StudentName,
    int CompletedCourses,
    int InProgressCourses,
    IReadOnlyList<EnrollmentProgressDto> Enrollments
) : IDto;
