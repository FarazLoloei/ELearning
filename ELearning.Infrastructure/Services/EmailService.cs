// <copyright file="EmailService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Services;

using ELearning.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);

        // In a real application, implement actual email sending logic
        return Task.CompletedTask;
    }

    public Task SendEnrollmentConfirmationAsync(string to, string studentName, string courseName)
    {
        var subject = $"Welcome to {courseName}";
        var body = $"Dear {studentName},\n\nThank you for enrolling in {courseName}. We hope you enjoy the course!\n\nBest regards,\nE-Learning Team";
        return this.SendEmailAsync(to, subject, body);
    }

    public Task SendAssignmentGradedAsync(string to, string studentName, string assignmentName, int score)
    {
        var subject = $"Your assignment {assignmentName} has been graded";
        var body = $"Dear {studentName},\n\nYour assignment {assignmentName} has been graded. You received {score} points.\n\nBest regards,\nE-Learning Team";
        return this.SendEmailAsync(to, subject, body);
    }

    public Task SendCourseApprovedAsync(string to, string instructorName, string courseName)
    {
        var subject = $"{courseName} is now published";
        var body = $"Dear {instructorName},\n\nYour course {courseName} has been approved and published.\n\nBest regards,\nE-Learning Team";
        return this.SendEmailAsync(to, subject, body);
    }

    public Task SendCourseRejectedAsync(string to, string instructorName, string courseName, string reason)
    {
        var subject = $"{courseName} needs changes before publication";
        var body = $"Dear {instructorName},\n\nYour course {courseName} was rejected during review.\nReason: {reason}\n\nYou can update the course and resubmit it for review.\n\nBest regards,\nE-Learning Team";
        return this.SendEmailAsync(to, subject, body);
    }

    public Task SendCertificateIssuedAsync(string to, string studentName, string courseName, string certificateCode)
    {
        var subject = $"Your certificate for {courseName} is ready";
        var body = $"Dear {studentName},\n\nCongratulations on completing {courseName}.\nYour certificate code is {certificateCode}.\n\nBest regards,\nE-Learning Team";
        return this.SendEmailAsync(to, subject, body);
    }
}
