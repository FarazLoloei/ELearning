// <copyright file="CreateSubmissionInput.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.InputTypes;

public record CreateSubmissionInput(
    Guid AssignmentId,
    string Content,
    string FileUrl);