// <copyright file="IEmailService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);

    Task SendEnrollmentConfirmationAsync(string to, string studentName, string courseName);

    Task SendAssignmentGradedAsync(string to, string studentName, string assignmentName, int score);
}