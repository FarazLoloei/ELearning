// <copyright file="ModuleConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Configurations;

using ELearning.Domain.Entities.CourseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(module => module.Id);

        builder.Property(module => module.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(module => module.Description)
            .IsRequired();

        builder.HasMany(module => module.Lessons)
            .WithOne()
            .HasForeignKey(lesson => lesson.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(module => module.Lessons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(module => module.Assignments)
            .WithOne()
            .HasForeignKey(assignment => assignment.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(module => module.Assignments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.ToTable("Modules");
    }
}
