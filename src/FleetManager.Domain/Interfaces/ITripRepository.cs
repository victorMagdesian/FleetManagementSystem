using FleetManager.Domain.Entities;

namespace FleetManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Trip entity operations.
/// Defines data access methods for trip management.
/// </summary>
public interface ITripRepository
{
    /// <summary>
    /// Retrieves a trip by its unique identifier.
    /// </summary>
    /// <param name="id">The trip's unique identifier</param>
    /// <returns>The trip if found, null otherwise</returns>
    Task<Trip?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all trips in the system.
    /// </summary>
    /// <returns>Collection of all trips</returns>
    Task<IEnumerable<Trip>> GetAllAsync();

    /// <summary>
    /// Retrieves all active trips (trips without an end date).
    /// </summary>
    /// <returns>Collection of active trips</returns>
    Task<IEnumerable<Trip>> GetActiveTripsAsync();

    /// <summary>
    /// Retrieves all trips for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The vehicle's unique identifier</param>
    /// <returns>Collection of trips for the vehicle</returns>
    Task<IEnumerable<Trip>> GetByVehicleIdAsync(Guid vehicleId);

    /// <summary>
    /// Retrieves all trips for a specific driver.
    /// </summary>
    /// <param name="driverId">The driver's unique identifier</param>
    /// <returns>Collection of trips for the driver</returns>
    Task<IEnumerable<Trip>> GetByDriverIdAsync(Guid driverId);

    /// <summary>
    /// Adds a new trip to the repository.
    /// </summary>
    /// <param name="trip">The trip to add</param>
    Task AddAsync(Trip trip);

    /// <summary>
    /// Updates an existing trip in the repository.
    /// </summary>
    /// <param name="trip">The trip to update</param>
    Task UpdateAsync(Trip trip);
}
