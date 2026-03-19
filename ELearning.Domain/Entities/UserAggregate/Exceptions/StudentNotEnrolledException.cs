// <copyright file="StudentNotEnrolledException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class StudentNotEnrolledException : DomainException
{
    public StudentNotEnrolledException(Guid studentId, Guid courseId)
        : base($"Student with ID '{studentId}' is not enrolled in course with ID '{courseId}'.")
    {
    }
}