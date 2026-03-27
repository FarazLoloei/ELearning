// <copyright file="DomainApplicationException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Exceptions;

public class DomainApplicationException : Exception
{
    public DomainApplicationException()
        : base()
    {
    }

    public DomainApplicationException(string message)
        : base(message)
    {
    }

    public DomainApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
