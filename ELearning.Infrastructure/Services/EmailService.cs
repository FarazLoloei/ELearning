using ELearning.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.Services;

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
        return SendEmailAsync(to, subject, body);
    }

    public Task SendAssignmentGradedAsync(string to, string studentName, string assignmentName, int score)
    {
        var subject = $"Your assignment {assignmentName} has been graded";
        var body = $"Dear {studentName},\n\nYour assignment {assignmentName} has been graded. You received {score} points.\n\nBest regards,\nE-Learning Team";
        return SendEmailAsync(to, subject, body);
    }
}