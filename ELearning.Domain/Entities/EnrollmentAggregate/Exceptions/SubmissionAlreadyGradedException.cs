// <copyright file="SubmissionAlreadyGradedException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class SubmissionAlreadyGradedException : DomainException
{
    public SubmissionAlreadyGradedException(Guid submissionId)
        : base($"Submission with ID '{submissionId}' has already been graded.")
    {
    }
}