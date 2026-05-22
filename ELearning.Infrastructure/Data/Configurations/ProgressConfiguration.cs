// <copyright file="ProgressConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProgressConfiguration : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.HasKey(progress => progress.Id);

        builder.Property(progress => progress.Status)
            .HasConversion(
                progressStatus => progressStatus.Id,
                id => ProgressStatus.GetAll<ProgressStatus>().Single(progressStatus => progressStatus.Id == id));

        builder.ToTable("Progresses");
    }
}
