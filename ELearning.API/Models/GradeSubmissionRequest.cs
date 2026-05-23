// <copyright file="GradeSubmissionRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public sealed class GradeSubmissionRequest
{
    public Guid SubmissionId { get; set; }

    public int Score { get; set; }

    public string Feedback { get; set; } = string.Empty;
}
