// <copyright file="VerifyCertificateQuery.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Queries;

using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Common.Model;
using MediatR;

public sealed record VerifyCertificateQuery(string CertificateCode) : IRequest<Result<CertificateDto>>;
