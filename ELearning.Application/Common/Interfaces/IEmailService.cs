// <copyright file="IEmailService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);

    Task SendEnrollmentConfirmationAsync(string to, string studentName, string courseName);

    Task SendAssignmentGradedAsync(string to, string studentName, string assignmentName, int score);

    Task SendCourseApprovedAsync(string to, string instructorName, string courseName);

    Task SendCourseRejectedAsync(string to, string instructorName, string courseName, string reason);

    Task SendCertificateIssuedAsync(string to, string studentName, string courseName, string certificateCode);
}
