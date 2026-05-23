// <copyright file="RejectCoursePublicationRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public sealed class RejectCoursePublicationRequest
{
    public Guid CourseId { get; set; }

    public string Reason { get; set; } = string.Empty;
}
