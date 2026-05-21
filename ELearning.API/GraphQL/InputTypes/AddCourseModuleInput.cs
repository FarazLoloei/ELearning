// <copyright file="AddCourseModuleInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public sealed record AddCourseModuleInput(
    Guid CourseId,
    string Title,
    string Description,
    int Order);
