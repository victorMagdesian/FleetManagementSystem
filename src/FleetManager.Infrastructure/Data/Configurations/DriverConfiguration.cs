using FleetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetManager.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Driver entity.
/// Defines property constraints and indexes.
/// </summary>
public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        // Primary key
        builder.HasKey(d => d.Id);

        // Property configurations
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LicenseNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(d => d.Active)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(d => d.LicenseNumber)
            .IsUnique()
            .HasDatabaseName("IX_Drivers_LicenseNumber");

        builder.HasIndex(d => d.Active)
            .HasDatabaseName("IX_Drivers_Active");

        // Relationships
        builder.HasMany(d => d.Trips)
            .WithOne(t => t.Driver)
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
