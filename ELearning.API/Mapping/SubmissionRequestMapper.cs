// <copyright file="SubmissionRequestMapper.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Mapping;

using ELearning.API.Models;
using ELearning.Application.Submissions.Commands;

internal static class SubmissionRequestMapper
{
    public static CreateSubmissionCommand ToCommand(this CreateSubmissionRequest request) =>
        new()
        {
            AssignmentId = request.AssignmentId,
            Content = request.Content ?? string.Empty,
            FileUrl = request.FileUrl ?? string.Empty,
        };

    public static GradeSubmissionCommand ToCommand(this GradeSubmissionRequest request) =>
        new()
        {
            SubmissionId = request.SubmissionId,
            Score = request.Score,
            Feedback = request.Feedback,
        };
}
