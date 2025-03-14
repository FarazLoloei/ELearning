﻿namespace ELearning.Application.Students.Dtos;

public readonly record struct EnrollmentProgressDto(
    Guid EnrollmentId,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage,
    int CompletedLessons,
    int TotalLessons,
    int CompletedAssignments,
    int TotalAssignments
);