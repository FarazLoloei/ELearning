// <copyright file="IDomainEvent.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Abstractions;

using MediatR;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUTC { get; }
}
