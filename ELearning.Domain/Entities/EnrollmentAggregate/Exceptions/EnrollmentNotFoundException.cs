﻿using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

public sealed class EnrollmentNotFoundException : DomainException
{
    public EnrollmentNotFoundException(Guid enrollmentId)
        : base($"Enrollment with ID '{enrollmentId}' was not found.")
    { }
}