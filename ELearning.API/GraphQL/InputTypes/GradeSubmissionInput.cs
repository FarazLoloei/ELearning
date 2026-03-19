// <copyright file="GradeSubmissionInput.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public record GradeSubmissionInput(
    Guid SubmissionId,
    int Score,
    string Feedback);