using FleetManager.Domain.Entities;

namespace FleetManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Vehicle entity operations.
/// Defines data access methods for vehicle management.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Retrieves a vehicle by its unique identifier.
    /// </summary>
    /// <param name="id">The vehicle's unique identifier</param>
    /// <returns>The vehicle if found, null otherwise</returns>
    Task<Vehicle?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all vehicles in the fleet.
    /// </summary>
    /// <returns>Collection of all vehicles</returns>
    Task<IEnumerable<Vehicle>> GetAllAsync();

    /// <summary>
    /// Retrieves all vehicles with Available status.
    /// </summary>
    /// <returns>Collection of available vehicles</returns>
    Task<IEnumerable<Vehicle>> GetAvailableAsync();

    /// <summary>
    /// Retrieves vehicles with upcoming maintenance within the specified threshold.
    /// </summary>
    /// <param name="daysThreshold">Number of days to look ahead for maintenance</param>
    /// <returns>Collection of vehicles with upcoming maintenance</returns>
    Task<IEnumerable<Vehicle>> GetVehiclesWithUpcomingMaintenanceAsync(int daysThreshold);

    /// <summary>
    /// Retrieves a vehicle by its license plate number.
    /// </summary>
    /// <param name="plate">The vehicle's license plate</param>
    /// <returns>The vehicle if found, null otherwise</returns>
    Task<Vehicle?> GetByPlateAsync(string plate);

    /// <summary>
    /// Adds a new vehicle to the repository.
    /// </summary>
    /// <param name="vehicle">The vehicle to add</param>
    Task AddAsync(Vehicle vehicle);

    /// <summary>
    /// Updates an existing vehicle in the repository.
    /// </summary>
    /// <param name="vehicle">The vehicle to update</param>
    Task UpdateAsync(Vehicle vehicle);

    /// <summary>
    /// Deletes a vehicle from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle to delete</param>
    Task DeleteAsync(Guid id);
}
