// <copyright file="AssignmentNotFoundException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class AssignmentNotFoundException : DomainException
{
    public AssignmentNotFoundException(Guid assignmentId)
        : base($"Assignment with ID '{assignmentId}' was not found.")
    {
    }
}