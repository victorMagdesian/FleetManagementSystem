namespace FleetManager.Domain.Entities;

/// <summary>
/// Represents a maintenance record for a vehicle.
/// Tracks maintenance activities including date, description, and cost.
/// </summary>
public class MaintenanceRecord
{
    /// <summary>
    /// Unique identifier for the maintenance record.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Foreign key to the associated vehicle.
    /// </summary>
    public Guid VehicleId { get; private set; }

    /// <summary>
    /// Date when the maintenance was performed.
    /// </summary>
    public DateTime Date { get; private set; }

    /// <summary>
    /// Description of the maintenance work performed.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Cost of the maintenance work.
    /// </summary>
    public decimal Cost { get; private set; }

    /// <summary>
    /// Navigation property to the associated vehicle.
    /// </summary>
    public Vehicle? Vehicle { get; private set; }

    // Private constructor for EF Core
    private MaintenanceRecord()
    {
        Description = string.Empty;
    }

    /// <summary>
    /// Creates a new maintenance record instance.
    /// </summary>
    /// <param name="vehicleId">ID of the vehicle being maintained</param>
    /// <param name="date">Date of maintenance</param>
    /// <param name="description">Description of maintenance work</param>
    /// <param name="cost">Cost of maintenance</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public MaintenanceRecord(Guid vehicleId, DateTime date, string description, decimal cost)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(vehicleId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (cost < 0)
            throw new ArgumentException("Cost cannot be negative", nameof(cost));

        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        Date = date;
        Description = description;
        Cost = cost;
    }
}
