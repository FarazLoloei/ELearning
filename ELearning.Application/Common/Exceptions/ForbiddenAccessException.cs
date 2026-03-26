// <copyright file="ForbiddenAccessException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Exceptions;

// Exception for forbidden access
public class ForbiddenAccessException : DomainApplicationException
{
    public ForbiddenAccessException()
        : base("You do not have permission to access this resource.")
    {
    }
}