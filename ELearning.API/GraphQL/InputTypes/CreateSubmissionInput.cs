// <copyright file="CreateSubmissionInput.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public record CreateSubmissionInput(
    Guid AssignmentId,
    string Content,
    string FileUrl);