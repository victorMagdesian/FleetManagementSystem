using FleetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Data;

/// <summary>
/// Database context for the FleetManager application.
/// Manages all entity sets and applies entity configurations.
/// </summary>
public class FleetManagerDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the FleetManagerDbContext.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext</param>
    public FleetManagerDbContext(DbContextOptions<FleetManagerDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Vehicles DbSet.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Drivers DbSet.
    /// </summary>
    public DbSet<Driver> Drivers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the MaintenanceRecords DbSet.
    /// </summary>
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Trips DbSet.
    /// </summary>
    public DbSet<Trip> Trips { get; set; } = null!;

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// Applies all entity configurations from the current assembly.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetManagerDbContext).Assembly);
    }
}
