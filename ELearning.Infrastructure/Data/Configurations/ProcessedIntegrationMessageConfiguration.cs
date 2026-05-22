// <copyright file="ProcessedIntegrationMessageConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProcessedIntegrationMessageConfiguration : IEntityTypeConfiguration<ProcessedIntegrationMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedIntegrationMessage> builder)
    {
        builder.HasKey(x => new { x.MessageId, x.Consumer });

        builder.Property(x => x.Consumer)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(4096);

        builder.HasIndex(x => x.ProcessedOnUtc);
    }
}
