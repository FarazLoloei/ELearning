using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Submissions.Dtos;

public readonly record struct SubmissionDto(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    DateTime SubmittedDate,
    bool IsGraded,
    int? Score,
    int MaxPoints
) : IDto;