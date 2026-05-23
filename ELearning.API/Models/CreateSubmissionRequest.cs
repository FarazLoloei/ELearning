// <copyright file="CreateSubmissionRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public sealed class CreateSubmissionRequest
{
    public Guid AssignmentId { get; set; }

    public string Content { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;
}
