// <copyright file="UpdateEnrollmentStatusRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public sealed class UpdateEnrollmentStatusRequest
{
    public Guid EnrollmentId { get; set; }

    public string Status { get; set; } = string.Empty;
}
