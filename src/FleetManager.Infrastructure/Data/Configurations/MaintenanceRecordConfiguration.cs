using FleetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetManager.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the MaintenanceRecord entity.
/// Defines property constraints and relationships.
/// </summary>
public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        // Primary key
        builder.HasKey(m => m.Id);

        // Property configurations
        builder.Property(m => m.VehicleId)
            .IsRequired();

        builder.Property(m => m.Date)
            .IsRequired();

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Cost)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        // Indexes
        builder.HasIndex(m => m.VehicleId)
            .HasDatabaseName("IX_MaintenanceRecords_VehicleId");

        // Relationships are configured in VehicleConfiguration
        // (HasMany/WithOne relationship from Vehicle side)
    }
}
