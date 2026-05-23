// <copyright file="CreateCourseRequest.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Models;

public sealed class CreateCourseRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int LevelId { get; set; }

    public decimal Price { get; set; }

    public int DurationHours { get; set; }

    public int DurationMinutes { get; set; }
}
