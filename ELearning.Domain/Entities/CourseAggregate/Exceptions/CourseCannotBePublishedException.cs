// <copyright file="CourseCannotBePublishedException.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

using ELearning.Domain.Exceptions;

public sealed class CourseCannotBePublishedException : DomainException
{
    public CourseCannotBePublishedException(string reason)
        : base($"Course cannot be published: '{reason}'")
    {
    }
}