// <copyright file="EnrollmentsController.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Controllers;

using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.API.Facades;
using ELearning.API.Models;
using ELearning.Application.Enrollments.Commands;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public class EnrollmentsController(IApiFacade apiFacade) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentDetailDto>>> GetEnrollment(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetEnrollmentDetailQuery { EnrollmentId = id };
        var result = await apiFacade.SendAsync(query, cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object?>>> CreateEnrollment(
        CreateEnrollmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.BadRequestResponse<object?>(result.Error);
        }

        return this.CreatedResponse();
    }

    [HttpPost("{id:guid}/lessons/{lessonId:guid}/start")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> StartLesson(
        Guid id,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new StartLessonCommand(id, lessonId), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost("{id:guid}/lessons/{lessonId:guid}/complete")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> CompleteLesson(
        Guid id,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new CompleteLessonCommand(id, lessonId), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> UpdateEnrollmentStatus(
        Guid id,
        UpdateEnrollmentStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.EnrollmentId)
        {
            return this.BadRequestResponse<object?>("Route id does not match payload EnrollmentId.");
        }

        var result = await apiFacade.SendAsync(command, cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpPost("{id:guid}/review")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> ReviewCourse(
        Guid id,
        ReviewCourseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(
            new ReviewCourseCommand(id, request.Rating, request.Review),
            cancellationToken);

        return this.FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }
}
