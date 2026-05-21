// <copyright file="AddCourseAssignmentInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed record AddCourseAssignmentInput(
    Guid CourseId,
    Guid ModuleId,
    string Title,
    string Description,
    int TypeId,
    int MaxPoints,
    DateTime? DueDate);
