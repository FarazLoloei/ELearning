// <copyright file="EnrollmentRequestMapper.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Mapping;

using ELearning.API.Models;
using ELearning.Application.Enrollments.Commands;

internal static class EnrollmentRequestMapper
{
    public static CreateEnrollmentCommand ToCommand(this CreateEnrollmentRequest request) =>
        new()
        {
            CourseId = request.CourseId,
        };

    public static UpdateEnrollmentStatusCommand ToCommand(this UpdateEnrollmentStatusRequest request) =>
        new()
        {
            EnrollmentId = request.EnrollmentId,
            Status = request.Status,
        };
}
