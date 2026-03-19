// <copyright file="ISubmissionReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Abstractions;

using ELearning.Application.Enrollments.ReadModels;
using ELearning.Application.Submissions.ReadModels;
using ELearning.SharedKernel.Abstractions;

public interface ISubmissionReadRepository : IReadRepository<SubmissionReadModel, Guid>
{
    Task<IReadOnlyList<SubmissionReadModel>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SubmissionReadModel>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task<SubmissionReadModel?> GetByStudentAndAssignmentIdAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SubmissionReadModel>> GetUngradedSubmissionsAsync(CancellationToken cancellationToken = default);
}