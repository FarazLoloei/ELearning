﻿namespace ELearning.Application.Common.Exceptions;

// Exception for forbidden access
public class ForbiddenAccessException : ApplicationException
{
    public ForbiddenAccessException() : base("You do not have permission to access this resource.")
    {
    }
}