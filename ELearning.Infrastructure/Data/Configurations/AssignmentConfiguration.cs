// <copyright file="AssignmentConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(assignment => assignment.Description)
            .IsRequired();

        builder.Property(assignment => assignment.Type)
            .HasConversion(
                assignmentType => assignmentType.Id,
                id => AssignmentType.GetAll<AssignmentType>().Single(assignmentType => assignmentType.Id == id));

        builder.ToTable("Assignments");
    }
}
