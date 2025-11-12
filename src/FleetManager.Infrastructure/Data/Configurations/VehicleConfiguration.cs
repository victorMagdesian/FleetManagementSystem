using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetManager.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Vehicle entity.
/// Defines property constraints, indexes, and relationships.
/// </summary>
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        // Primary key
        builder.HasKey(v => v.Id);

        // Property configurations
        builder.Property(v => v.Plate)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Year)
            .IsRequired();

        builder.Property(v => v.Mileage)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(v => v.LastMaintenanceDate)
            .IsRequired();

        builder.Property(v => v.NextMaintenanceDate)
            .IsRequired();

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(VehicleStatus.Available);

        // Indexes
        builder.HasIndex(v => v.Plate)
            .IsUnique()
            .HasDatabaseName("IX_Vehicles_Plate");

        builder.HasIndex(v => v.Status)
            .HasDatabaseName("IX_Vehicles_Status");

        builder.HasIndex(v => v.NextMaintenanceDate)
            .HasDatabaseName("IX_Vehicles_NextMaintenanceDate");

        // Relationships
        builder.HasMany(v => v.MaintenanceRecords)
            .WithOne(m => m.Vehicle)
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Trips)
            .WithOne(t => t.Vehicle)
            .HasForeignKey(t => t.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
