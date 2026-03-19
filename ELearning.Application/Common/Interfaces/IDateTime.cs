// <copyright file="IDateTime.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}