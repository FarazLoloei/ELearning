// <copyright file="AddCourseLessonRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

using System.ComponentModel.DataAnnotations;

public sealed class AddCourseLessonRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    [Range(0, int.MaxValue)]
    public int DurationHours { get; set; }

    [Range(0, 59)]
    public int DurationMinutes { get; set; }

    [MaxLength(500)]
    public string? VideoUrl { get; set; }
}
