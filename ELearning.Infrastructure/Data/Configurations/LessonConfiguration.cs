// <copyright file="LessonConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(lesson => lesson.Id);

        builder.Property(lesson => lesson.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(lesson => lesson.Content)
            .IsRequired();

        builder.Property(lesson => lesson.VideoUrl)
            .HasMaxLength(500);

        builder.Property(lesson => lesson.Type)
            .HasConversion(
                lessonType => lessonType.Id,
                id => LessonType.GetAll<LessonType>().Single(lessonType => lessonType.Id == id));

        builder.OwnsOne(lesson => lesson.Duration, duration =>
        {
            duration.Property(value => value.Hours).HasColumnName("DurationHours");
            duration.Property(value => value.Minutes).HasColumnName("DurationMinutes");
        });
    }
}
