﻿using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

public sealed class SubmissionNotFoundException : DomainException
{
    public SubmissionNotFoundException(Guid submissionId)
        : base($"Submission with ID '{submissionId}' was not found.")
    { }
}