// <copyright file="IInstructorReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

using ELearning.Application.Instructors.ReadModels;
using ELearning.SharedKernel.Abstractions;

public interface IInstructorReadRepository : IReadRepository<InstructorReadModel, Guid>
{
    Task<InstructorWithCoursesReadModel?> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken = default);

    Task<int> GetTotalStudentCountAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<decimal> GetAverageRatingAsync(Guid instructorId, CancellationToken cancellationToken);
}