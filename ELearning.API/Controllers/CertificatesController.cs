// <copyright file="CertificatesController.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Controllers;

using Asp.Versioning;
using ELearning.API.Contracts;
using ELearning.API.Facades;
using ELearning.Application.Certificates.Commands;
using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Certificates.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
public sealed class CertificatesController(IApiFacade apiFacade) : ApiControllerBase
{
    [HttpPost("enrollments/{enrollmentId:guid}/issue")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CertificateDto>>> IssueForEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(
            new IssueCertificateForCompletedEnrollmentCommand(enrollmentId),
            cancellationToken);

        return this.FromResult(result, error => error.StartsWith("Enrollment not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("enrollments/{enrollmentId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CertificateDto>>> GetForEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new GetEnrollmentCertificateQuery(enrollmentId), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Certificate not found", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("verify/{certificateCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CertificateDto>>> Verify(
        string certificateCode,
        CancellationToken cancellationToken)
    {
        var result = await apiFacade.SendAsync(new VerifyCertificateQuery(certificateCode), cancellationToken);
        return this.FromResult(result, error => error.StartsWith("Certificate not found", StringComparison.OrdinalIgnoreCase));
    }
}
