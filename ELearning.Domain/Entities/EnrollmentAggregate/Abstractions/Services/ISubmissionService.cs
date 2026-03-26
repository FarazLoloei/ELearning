// <copyright file="ISubmissionService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Services;

public interface ISubmissionService
{
    Task<bool> CanGradeSubmissionAsync(Guid instructorId, Guid submissionId);

    Task NotifyStudentOfGradedSubmissionAsync(Submission submission);
}