using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearning.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        // Configure value objects
        builder.OwnsOne(u => u.Email, nb =>
        {
            nb.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(254)
                .IsRequired();
            nb.HasIndex(e => e.Value).IsUnique();
        });

        // Configure enumerations
        builder.Property(u => u.Role)
            .HasConversion(
                v => v.Id,
                v => UserRole.GetAll<UserRole>().Single(e => e.Id == v));

        // Table name and TPH inheritance
        builder.HasDiscriminator<string>("UserType")
            .HasValue<Student>("Student")
            .HasValue<Instructor>("Instructor");
    }
}