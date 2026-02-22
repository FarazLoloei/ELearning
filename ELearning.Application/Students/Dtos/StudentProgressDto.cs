using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Dtos;

public readonly record struct StudentProgressDto(
    Guid StudentId,
    string StudentName,
    int CompletedCourses,
    int InProgressCourses,
    List<EnrollmentProgressDto> Enrollments
) : IDto;