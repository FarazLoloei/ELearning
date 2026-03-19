// <copyright file="IDomainEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Abstractions;

using MediatR;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUTC { get; }
}
