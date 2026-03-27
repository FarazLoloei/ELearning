// <copyright file="IDateTime.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}