// <copyright file="INotificationRequestService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

public interface INotificationRequestService
{
    Task RequestEnrollmentConfirmationAsync(
        string recipientEmail,
        string studentName,
        string courseName,
        Guid sourceId,
        CancellationToken cancellationToken);

    Task RequestAssignmentGradedAsync(
        string recipientEmail,
        string studentName,
        string assignmentName,
        int score,
        Guid sourceId,
        CancellationToken cancellationToken);

    Task RequestCourseApprovedAsync(
        string recipientEmail,
        string instructorName,
        string courseName,
        Guid sourceId,
        CancellationToken cancellationToken);

    Task RequestCourseRejectedAsync(
        string recipientEmail,
        string instructorName,
        string courseName,
        string reason,
        Guid sourceId,
        CancellationToken cancellationToken);

    Task RequestCertificateIssuedAsync(
        string recipientEmail,
        string studentName,
        string courseName,
        string certificateCode,
        Guid sourceId,
        CancellationToken cancellationToken);
}
