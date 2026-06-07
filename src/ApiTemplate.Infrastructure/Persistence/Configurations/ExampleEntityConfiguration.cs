using ApiTemplate.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiTemplate.Infrastructure.Persistence.Configurations;

/// <summary>EF Core Fluent API configuration for the <see cref="Example"/> entity.</summary>
public class ExampleEntityConfiguration : IEntityTypeConfiguration<Example>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(e => e.Title).IsUnique();

        builder.HasOne<AppUser>(e => e.CreatedByUser)
            .WithMany(u => u.Examples)
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
