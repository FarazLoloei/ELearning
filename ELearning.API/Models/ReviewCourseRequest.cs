// <copyright file="ReviewCourseRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

using System.ComponentModel.DataAnnotations;

public sealed class ReviewCourseRequest
{
    [Range(1, 5)]
    public decimal Rating { get; set; }

    [MaxLength(1000)]
    public string? Review { get; set; }
}
