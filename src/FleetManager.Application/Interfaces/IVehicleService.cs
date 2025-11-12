using FleetManager.Application.DTOs;

namespace FleetManager.Application.Interfaces;

/// <summary>
/// Service interface for vehicle management operations.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Retrieves a vehicle by its unique identifier.
    /// </summary>
    /// <param name="id">The vehicle's unique identifier</param>
    /// <returns>Vehicle response DTO</returns>
    /// <exception cref="Exceptions.EntityNotFoundException">Thrown when vehicle is not found</exception>
    Task<VehicleResponse> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all vehicles in the fleet.
    /// </summary>
    /// <returns>Collection of vehicle response DTOs</returns>
    Task<IEnumerable<VehicleResponse>> GetAllAsync();

    /// <summary>
    /// Retrieves all available vehicles.
    /// </summary>
    /// <returns>Collection of available vehicle response DTOs</returns>
    Task<IEnumerable<VehicleResponse>> GetAvailableAsync();

    /// <summary>
    /// Retrieves vehicles with upcoming maintenance within the specified threshold.
    /// </summary>
    /// <param name="daysThreshold">Number of days to look ahead for maintenance</param>
    /// <returns>Collection of vehicle response DTOs with upcoming maintenance</returns>
    Task<IEnumerable<VehicleResponse>> GetUpcomingMaintenanceAsync(int daysThreshold);

    /// <summary>
    /// Creates a new vehicle.
    /// </summary>
    /// <param name="request">Vehicle creation request</param>
    /// <returns>Created vehicle response DTO</returns>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when plate already exists</exception>
    Task<VehicleResponse> CreateAsync(CreateVehicleRequest request);

    /// <summary>
    /// Updates an existing vehicle.
    /// </summary>
    /// <param name="id">The vehicle's unique identifier</param>
    /// <param name="request">Vehicle update request</param>
    /// <returns>Updated vehicle response DTO</returns>
    /// <exception cref="Exceptions.EntityNotFoundException">Thrown when vehicle is not found</exception>
    Task<VehicleResponse> UpdateAsync(Guid id, UpdateVehicleRequest request);

    /// <summary>
    /// Deletes a vehicle.
    /// </summary>
    /// <param name="id">The vehicle's unique identifier</param>
    /// <exception cref="Exceptions.EntityNotFoundException">Thrown when vehicle is not found</exception>
    Task DeleteAsync(Guid id);
}
