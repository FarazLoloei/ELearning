// <copyright file="AddCourseLessonInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed record AddCourseLessonInput(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    string Content,
    int TypeId,
    int Order,
    int DurationHours,
    int DurationMinutes,
    string? VideoUrl);
