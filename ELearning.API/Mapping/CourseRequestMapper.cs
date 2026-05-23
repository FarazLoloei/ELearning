// <copyright file="CourseRequestMapper.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Mapping;

using ELearning.API.Models;
using ELearning.Application.Courses.Commands;

internal static class CourseRequestMapper
{
    public static CreateCourseCommand ToCommand(this CreateCourseRequest request) =>
        new(
            request.Title,
            request.Description,
            request.CategoryId,
            request.LevelId,
            request.Price,
            request.DurationHours,
            request.DurationMinutes);

    public static UpdateCourseCommand ToCommand(this UpdateCourseRequest request) =>
        new(
            request.CourseId,
            request.Title,
            request.Description,
            request.CategoryId,
            request.LevelId,
            request.Price,
            request.DurationHours,
            request.DurationMinutes,
            request.IsFeatured);

    public static RejectCoursePublicationCommand ToCommand(this RejectCoursePublicationRequest request) =>
        new(request.CourseId, request.Reason);
}
