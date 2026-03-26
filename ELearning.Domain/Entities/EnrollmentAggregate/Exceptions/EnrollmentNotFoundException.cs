// <copyright file="EnrollmentNotFoundException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class EnrollmentNotFoundException : DomainException
{
    public EnrollmentNotFoundException(Guid enrollmentId)
        : base($"Enrollment with ID '{enrollmentId}' was not found.")
    {
    }
}