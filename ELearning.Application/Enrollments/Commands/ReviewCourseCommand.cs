// <copyright file="ReviewCourseCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record ReviewCourseCommand(Guid EnrollmentId, decimal Rating, string? Review) : IRequest<Result>;
