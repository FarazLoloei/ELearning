// <copyright file="DomainException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Exceptions;

[Serializable]
public class DomainException : Exception
{
    public string? ErrorCode { get; }

    public string? Details { get; }

    public DomainException()
    {
    }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DomainException(string message, string errorCode)
        : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public DomainException(string message, string errorCode, string details)
        : base(message)
    {
        this.ErrorCode = errorCode;
        this.Details = details;
    }
}
