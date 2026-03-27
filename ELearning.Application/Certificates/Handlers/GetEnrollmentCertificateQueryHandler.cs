// <copyright file="GetEnrollmentCertificateQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Handlers;

using ELearning.Application.Certificates.Abstractions;
using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Certificates.Queries;
using ELearning.Application.Certificates.ReadModels;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using MediatR;

public sealed class GetEnrollmentCertificateQueryHandler(
        ICertificateReadRepository certificateReadRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetEnrollmentCertificateQuery, Result<CertificateDto>>
{
    public async Task<Result<CertificateDto>> Handle(GetEnrollmentCertificateQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var certificate = await certificateReadRepository.GetByEnrollmentIdAsync(request.EnrollmentId, cancellationToken);
        if (certificate is null)
        {
            return Result.Failure<CertificateDto>("Certificate not found for enrollment.");
        }

        var isOwner = certificate.StudentId == currentUserService.UserId.Value;
        if (!isOwner && !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }

        return Result.Success(MapToDto(certificate));
    }

    private static CertificateDto MapToDto(CertificateReadModel certificate) =>
        new(
            certificate.Id,
            certificate.CertificateCode,
            certificate.EnrollmentId,
            certificate.StudentId,
            certificate.StudentName,
            certificate.CourseId,
            certificate.CourseTitle,
            certificate.IssuedOnUtc);
}
