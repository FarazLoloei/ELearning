// <copyright file="CertificateReadModel.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.ReadModels;

public sealed record CertificateReadModel(
    Guid Id,
    string CertificateCode,
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    DateTime IssuedOnUtc);
