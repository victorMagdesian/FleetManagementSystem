using FleetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetManager.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Trip entity.
/// Defines property constraints and relationships.
/// </summary>
public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        // Primary key
        builder.HasKey(t => t.Id);

        // Property configurations
        builder.Property(t => t.VehicleId)
            .IsRequired();

        builder.Property(t => t.DriverId)
            .IsRequired();

        builder.Property(t => t.Route)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.StartDate)
            .IsRequired();

        builder.Property(t => t.EndDate)
            .IsRequired(false); // Nullable - trip may be active

        builder.Property(t => t.Distance)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(t => t.VehicleId)
            .HasDatabaseName("IX_Trips_VehicleId");

        builder.HasIndex(t => t.DriverId)
            .HasDatabaseName("IX_Trips_DriverId");

        builder.HasIndex(t => t.EndDate)
            .HasDatabaseName("IX_Trips_EndDate");

        // Relationships are configured in VehicleConfiguration and DriverConfiguration
        // (HasMany/WithOne relationships from Vehicle and Driver sides)
    }
}
