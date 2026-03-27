// <copyright file="GetEnrollmentCertificateQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Queries;

using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Common.Model;
using MediatR;

public sealed record GetEnrollmentCertificateQuery(Guid EnrollmentId) : IRequest<Result<CertificateDto>>;
