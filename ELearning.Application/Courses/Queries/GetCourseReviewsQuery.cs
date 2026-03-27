// <copyright file="GetCourseReviewsQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Queries;

using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using MediatR;

public sealed record GetCourseReviewsQuery(Guid CourseId) : IRequest<Result<IReadOnlyList<ReviewDto>>>;
