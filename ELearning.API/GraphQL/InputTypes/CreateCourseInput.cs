// <copyright file="CreateCourseInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public record CreateCourseInput(
    string Title,
    string Description,
    int CategoryId,
    int LevelId,
    decimal Price,
    int DurationHours,
    int DurationMinutes);