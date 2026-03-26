// <copyright file="ILessonReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Abstractions;

using ELearning.Application.Courses.ReadModels;
using ELearning.SharedKernel.Abstractions;

public interface ILessonReadRepository : IReadRepository<LessonReadModel, Guid>
{
    Task<IReadOnlyList<LessonReadModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
}