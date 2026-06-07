using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiTemplate.Infrastructure.Persistence.Configurations;

/// <summary>EF Core Fluent API configuration for the <see cref="AppUser"/> entity.</summary>
public class UserEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();
    }
}
