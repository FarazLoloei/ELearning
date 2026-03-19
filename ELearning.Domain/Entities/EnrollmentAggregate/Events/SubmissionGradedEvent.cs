// <copyright file="SubmissionGradedEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Events;

using ELearning.SharedKernel.Abstractions;

public sealed class SubmissionGradedEvent : IDomainEvent
{
    public Submission Submission { get; }

    public DateTime OccurredOnUTC { get; }

    public SubmissionGradedEvent(Submission submission)
    {
        this.Submission = submission ?? throw new ArgumentNullException(nameof(submission));
        this.OccurredOnUTC = DateTime.UtcNow;
    }
}