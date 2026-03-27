// <copyright file="VerifyCertificateQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Handlers;

using ELearning.Application.Certificates.Abstractions;
using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Certificates.Queries;
using ELearning.Application.Common.Model;
using MediatR;

public sealed class VerifyCertificateQueryHandler(ICertificateReadRepository certificateReadRepository)
    : IRequestHandler<VerifyCertificateQuery, Result<CertificateDto>>
{
    public async Task<Result<CertificateDto>> Handle(VerifyCertificateQuery request, CancellationToken cancellationToken)
    {
        var certificate = await certificateReadRepository.GetByCodeAsync(request.CertificateCode, cancellationToken);
        if (certificate is null)
        {
            return Result.Failure<CertificateDto>("Certificate not found.");
        }

        return Result.Success(new CertificateDto(
            certificate.Id,
            certificate.CertificateCode,
            certificate.EnrollmentId,
            certificate.StudentId,
            certificate.StudentName,
            certificate.CourseId,
            certificate.CourseTitle,
            certificate.IssuedOnUtc));
    }
}
