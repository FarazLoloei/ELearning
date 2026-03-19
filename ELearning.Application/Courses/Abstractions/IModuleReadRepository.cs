// <copyright file="IModuleReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Abstractions;

using ELearning.SharedKernel.Abstractions;

public interface IModuleReadRepository : IReadRepository<ModuleReadModel, Guid>
{
    Task<IReadOnlyList<ModuleReadModel>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
}