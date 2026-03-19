// <copyright file="StudentAlreadyEnrolledException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class StudentAlreadyEnrolledException : DomainException
{
    public StudentAlreadyEnrolledException(Guid studentId, Guid courseId)
        : base($"Student with ID '{studentId}' is already enrolled in course with ID {courseId}.")
    {
    }
}