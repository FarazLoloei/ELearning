// <copyright file="CertificateConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Domain.Entities.CertificateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificateCode)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(c => c.IssuedOnUtc)
            .IsRequired();

        builder.HasIndex(c => c.CertificateCode)
            .IsUnique();

        builder.HasIndex(c => c.EnrollmentId)
            .IsUnique();

        builder.ToTable("Certificates");
    }
}
