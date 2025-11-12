namespace FleetManager.Domain.Entities;

/// <summary>
/// Represents a trip made by a vehicle with a driver.
/// Tracks route, dates, and distance traveled.
/// </summary>
public class Trip
{
    /// <summary>
    /// Unique identifier for the trip.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Foreign key to the associated vehicle.
    /// </summary>
    public Guid VehicleId { get; private set; }

    /// <summary>
    /// Foreign key to the associated driver.
    /// </summary>
    public Guid DriverId { get; private set; }

    /// <summary>
    /// Route or destination description for the trip.
    /// </summary>
    public string Route { get; private set; }

    /// <summary>
    /// Date and time when the trip started.
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Date and time when the trip ended (null if trip is still active).
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// Distance traveled during the trip in kilometers.
    /// </summary>
    public int Distance { get; private set; }

    /// <summary>
    /// Navigation property to the associated vehicle.
    /// </summary>
    public Vehicle? Vehicle { get; private set; }

    /// <summary>
    /// Navigation property to the associated driver.
    /// </summary>
    public Driver? Driver { get; private set; }

    // Private constructor for EF Core
    private Trip()
    {
        Route = string.Empty;
    }

    /// <summary>
    /// Creates a new trip instance.
    /// </summary>
    /// <param name="vehicleId">ID of the vehicle for this trip</param>
    /// <param name="driverId">ID of the driver for this trip</param>
    /// <param name="route">Route or destination description</param>
    /// <param name="startDate">Date and time when the trip starts</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public Trip(Guid vehicleId, Guid driverId, string route, DateTime startDate)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(vehicleId));

        if (driverId == Guid.Empty)
            throw new ArgumentException("Driver ID cannot be empty", nameof(driverId));

        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route cannot be empty", nameof(route));

        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        DriverId = driverId;
        Route = route;
        StartDate = startDate;
        EndDate = null; // Trip starts as active
        Distance = 0; // Distance is set when trip ends
    }

    /// <summary>
    /// Finalizes the trip by setting the end date and distance traveled.
    /// </summary>
    /// <param name="distance">Distance traveled during the trip in kilometers</param>
    /// <exception cref="InvalidOperationException">Thrown when trip is already ended</exception>
    /// <exception cref="ArgumentException">Thrown when distance is negative</exception>
    public void End(int distance)
    {
        if (EndDate.HasValue)
            throw new InvalidOperationException("Trip has already been ended");

        if (distance < 0)
            throw new ArgumentException("Distance cannot be negative", nameof(distance));

        EndDate = DateTime.UtcNow;
        Distance = distance;
    }

    /// <summary>
    /// Checks if the trip is currently active (not yet ended).
    /// </summary>
    /// <returns>True if the trip is active (EndDate is null), false otherwise</returns>
    public bool IsActive()
    {
        return !EndDate.HasValue;
    }
}
