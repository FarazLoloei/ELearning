// <copyright file="CertificatePayload.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.Payloads;

using ELearning.Application.Certificates.Dtos;

[GraphQLDescription("Payload for certificate operations")]
public sealed class CertificatePayload : PayloadBase
{
    [GraphQLDescription("The certificate returned by the operation")]
    public CertificateDto? Certificate { get; }

    public CertificatePayload()
    {
    }

    public CertificatePayload(CertificateDto certificate) => this.Certificate = certificate;

    public CertificatePayload(string error)
        : base(new Error("CERTIFICATE_ERROR", error))
    {
    }

    public CertificatePayload(Error error)
        : base(error)
    {
    }
}
