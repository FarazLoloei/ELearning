// <copyright file="SubmissionReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.ReadModels;

public sealed record SubmissionReadModel(
    Guid Id,
    Guid EnrollmentId,
    Guid AssignmentId,
    string Content,
    string FileUrl,
    bool IsGraded,
    int? Score,
    string Feedback,
    DateTime SubmittedDate,
    Guid? GradedById,
    DateTime? GradedDate);