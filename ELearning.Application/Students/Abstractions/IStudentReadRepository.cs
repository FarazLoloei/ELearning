// <copyright file="IStudentReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Abstractions;

using ELearning.Application.Students.ReadModels;
using ELearning.SharedKernel.Abstractions;

public interface IStudentReadRepository : IReadRepository<StudentReadModel, Guid>
{
    // Task<StudentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentReadModel>> GetStudentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentCourseReadModel>> GetCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);

    Task<int> GetEnrolledStudentCountAsync(Guid courseId, CancellationToken cancellationToken);
}