// <copyright file="DeleteCourseCommand.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

/// <summary>
/// Command to delete a course.
/// </summary>
public record DeleteCourseCommand(Guid CourseId) : IRequest<Result>;