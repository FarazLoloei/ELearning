// <copyright file="ArchiveCourseCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record ArchiveCourseCommand(Guid CourseId) : IRequest<Result>;
