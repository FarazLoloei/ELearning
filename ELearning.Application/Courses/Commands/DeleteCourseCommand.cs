// <copyright file="DeleteCourseCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

/// <summary>
/// Command to delete a course.
/// </summary>
public record DeleteCourseCommand(Guid CourseId) : IRequest<Result>;