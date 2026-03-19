// <copyright file="SubmissionDetailDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Dtos;

using ELearning.SharedKernel.Abstractions;

// Detailed DTO for submission
public sealed record SubmissionDetailDto(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    DateTime SubmittedDate,
    bool IsGraded,
    int? Score,
    int MaxPoints,
    Guid StudentId,
    string StudentName,
    string Content,
    string FileUrl,
    string Feedback,
    Guid? GradedById,
    string GradedByName,
    DateTime? GradedDate) : IDto;
