// <copyright file="IProgressReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Abstractions;

using ELearning.Application.Enrollments.ReadModels;
using ELearning.SharedKernel.Abstractions;

public interface IProgressReadRepository : IReadRepository<ProgressReadModel, Guid>
{
    Task<IReadOnlyList<ProgressReadModel>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<ProgressReadModel?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);

    Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}