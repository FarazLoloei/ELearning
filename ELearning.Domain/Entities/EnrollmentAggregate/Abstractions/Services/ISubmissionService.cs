// <copyright file="ISubmissionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Services;

public interface ISubmissionService
{
    Task<bool> CanGradeSubmissionAsync(Guid instructorId, Guid submissionId);

    Task NotifyStudentOfGradedSubmissionAsync(Submission submission);
}