// <copyright file="RejectCoursePublicationCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands;

using ELearning.Application.Common.Model;
using MediatR;

public sealed record RejectCoursePublicationCommand(Guid CourseId, string Reason) : IRequest<Result>;
