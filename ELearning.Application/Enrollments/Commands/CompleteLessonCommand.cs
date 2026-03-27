// <copyright file="CompleteLessonCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record CompleteLessonCommand(Guid EnrollmentId, Guid LessonId) : IRequest<Result>;
