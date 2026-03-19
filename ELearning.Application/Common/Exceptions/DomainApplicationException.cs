// <copyright file="DomainApplicationException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
