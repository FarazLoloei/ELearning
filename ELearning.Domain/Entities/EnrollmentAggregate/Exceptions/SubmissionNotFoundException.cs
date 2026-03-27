// <copyright file="SubmissionNotFoundException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class SubmissionNotFoundException : DomainException
{
    public SubmissionNotFoundException(Guid submissionId)
        : base($"Submission with ID '{submissionId}' was not found.")
    {
    }
}