using ELearning.Application.Submissions.Dtos;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Enrollments.Dtos;

public sealed record EnrollmentDetailDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage,
    IReadOnlyList<LessonProgressDto> LessonProgress,
    IReadOnlyList<SubmissionDto> Submissions,
    decimal? CourseRating,
    string? Review
) : IDto;
