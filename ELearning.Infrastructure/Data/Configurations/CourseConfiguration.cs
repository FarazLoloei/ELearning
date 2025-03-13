using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearning.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .IsRequired();

        // Configure value objects
        builder.OwnsOne(c => c.Duration, nb =>
        {
            nb.Property(d => d.Hours).HasColumnName("DurationHours");
            nb.Property(d => d.Minutes).HasColumnName("DurationMinutes");
        });

        builder.OwnsOne(c => c.AverageRating, nb =>
        {
            nb.Property(r => r.Value).HasColumnName("AverageRatingValue");
            nb.Property(r => r.NumberOfRatings).HasColumnName("NumberOfRatings");
        });

        // Configure enumerations
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.Id,
                v => CourseStatus.GetAll<CourseStatus>().Single(e => e.Id == v));

        builder.Property(c => c.Level)
            .HasConversion(
                v => v.Id,
                v => CourseLevel.GetAll<CourseLevel>().Single(e => e.Id == v));

        builder.Property(c => c.Category)
            .HasConversion(
                v => v.Id,
                v => CourseCategory.GetAll<CourseCategory>().Single(e => e.Id == v));

        // Configure relationships
        builder.HasOne<Instructor>()
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Modules)
            .WithOne()
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Enrollments)
            .WithOne()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Table name
        builder.ToTable("Courses");
    }
}