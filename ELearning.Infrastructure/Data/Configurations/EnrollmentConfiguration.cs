// <copyright file="EnrollmentConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Configure enumerations
        builder.Property(e => e.Status)
            .HasConversion(
                v => v.Id,
                v => EnrollmentStatus.GetAll<EnrollmentStatus>().Single(s => s.Id == v));

        // Configure value objects
        builder.OwnsOne(e => e.CourseRating, nb =>
        {
            nb.Property(r => r.Value).HasColumnName("CourseRatingValue");
            nb.Property(r => r.NumberOfRatings).HasColumnName("CourseRatingCount");
        });

        // Configure relationships
        builder.HasOne<Student>()
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ProgressRecords)
            .WithOne()
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Submissions)
            .WithOne()
            .HasForeignKey(s => s.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
