// <copyright file="EmailAlreadyExistsException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.")
    {
    }
}