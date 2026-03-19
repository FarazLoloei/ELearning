// <copyright file="ModuleNotFoundException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class ModuleNotFoundException : DomainException
{
    public ModuleNotFoundException(Guid moduleId)
        : base($"Module with ID '{moduleId}' was not found.")
    {
    }
}