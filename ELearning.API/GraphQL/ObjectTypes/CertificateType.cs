// <copyright file="CertificateType.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL.ObjectTypes;

using ELearning.Application.Certificates.Dtos;

public sealed class CertificateType : ObjectType<CertificateDto>
{
    protected override void Configure(IObjectTypeDescriptor<CertificateDto> descriptor)
    {
        descriptor.Description("Represents a course completion certificate.");

        descriptor.Field(c => c.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.CertificateCode).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.EnrollmentId).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.StudentId).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.StudentName).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.CourseId).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.CourseTitle).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.IssuedOnUtc).Type<NonNullType<DateTimeType>>();
    }
}
