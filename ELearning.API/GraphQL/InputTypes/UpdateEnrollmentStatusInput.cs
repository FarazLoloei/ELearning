// <copyright file="UpdateEnrollmentStatusInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed class UpdateEnrollmentStatusInput
{
    public Guid EnrollmentId { get; init; }

    public string Status { get; init; } = string.Empty;
}
