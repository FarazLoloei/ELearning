// <copyright file="AddCourseAssignmentRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

using System.ComponentModel.DataAnnotations;

public sealed class AddCourseAssignmentRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxPoints { get; set; }

    public DateTime? DueDate { get; set; }
}
