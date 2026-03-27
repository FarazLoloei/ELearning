// <copyright file="GradeSubmissionInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public record GradeSubmissionInput(
    Guid SubmissionId,
    int Score,
    string Feedback);