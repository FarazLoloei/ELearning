// <copyright file="ICourseReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Abstractions;

using ELearning.Application.Courses.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

public interface ICourseReadRepository : IReadRepository<CourseReadModel, Guid>
{
    Task<PaginatedList<CourseReadModel>> SearchAsync(
        string? searchTerm,
        int? categoryId,
        int? levelId,
        bool? isFeatured,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CourseReviewReadModel>> GetReviewsByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken = default);
}
