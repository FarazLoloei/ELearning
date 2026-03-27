// <copyright file="CertificateDto.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Dtos;

using ELearning.SharedKernel.Abstractions;

public sealed record CertificateDto(
    Guid Id,
    string CertificateCode,
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    DateTime IssuedOnUtc) : IDto;
