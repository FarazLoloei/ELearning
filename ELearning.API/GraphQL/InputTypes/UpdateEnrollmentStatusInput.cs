// <copyright file="UpdateEnrollmentStatusInput.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed class UpdateEnrollmentStatusInput
{
    public Guid EnrollmentId { get; init; }

    public string Status { get; init; } = string.Empty;
}
