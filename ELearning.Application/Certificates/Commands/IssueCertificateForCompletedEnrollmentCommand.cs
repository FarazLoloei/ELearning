// <copyright file="IssueCertificateForCompletedEnrollmentCommand.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Commands;

using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Common.Model;
using MediatR;

public sealed record IssueCertificateForCompletedEnrollmentCommand(Guid EnrollmentId) : IRequest<Result<CertificateDto>>;
