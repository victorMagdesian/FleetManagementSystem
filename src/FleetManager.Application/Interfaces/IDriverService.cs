using FleetManager.Application.DTOs;

namespace FleetManager.Application.Interfaces;

/// <summary>
/// Service interface for driver management operations.
/// </summary>
public interface IDriverService
{
    /// <summary>
    /// Retrieves a driver by their unique identifier.
    /// </summary>
    /// <param name="id">The driver's unique identifier</param>
    /// <returns>The driver response DTO</returns>
    /// <exception cref="EntityNotFoundException">Thrown when driver is not found</exception>
    Task<DriverResponse> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all drivers in the system.
    /// </summary>
    /// <returns>Collection of all driver response DTOs</returns>
    Task<IEnumerable<DriverResponse>> GetAllAsync();

    /// <summary>
    /// Retrieves all active and available drivers.
    /// </summary>
    /// <returns>Collection of available driver response DTOs</returns>
    Task<IEnumerable<DriverResponse>> GetAvailableAsync();

    /// <summary>
    /// Creates a new driver with license number uniqueness validation.
    /// </summary>
    /// <param name="request">The driver creation request</param>
    /// <returns>The created driver response DTO</returns>
    /// <exception cref="DuplicateEntityException">Thrown when license number already exists</exception>
    Task<DriverResponse> CreateAsync(CreateDriverRequest request);

    /// <summary>
    /// Updates an existing driver's information.
    /// </summary>
    /// <param name="id">The driver's unique identifier</param>
    /// <param name="request">The driver update request</param>
    /// <returns>The updated driver response DTO</returns>
    /// <exception cref="EntityNotFoundException">Thrown when driver is not found</exception>
    Task<DriverResponse> UpdateAsync(Guid id, CreateDriverRequest request);

    /// <summary>
    /// Deletes a driver from the system.
    /// </summary>
    /// <param name="id">The driver's unique identifier</param>
    /// <exception cref="EntityNotFoundException">Thrown when driver is not found</exception>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Activates a driver, allowing them to be assigned to trips.
    /// </summary>
    /// <param name="id">The driver's unique identifier</param>
    /// <returns>The updated driver response DTO</returns>
    /// <exception cref="EntityNotFoundException">Thrown when driver is not found</exception>
    Task<DriverResponse> ActivateAsync(Guid id);

    /// <summary>
    /// Deactivates a driver, preventing them from being assigned to new trips.
    /// </summary>
    /// <param name="id">The driver's unique identifier</param>
    /// <returns>The updated driver response DTO</returns>
    /// <exception cref="EntityNotFoundException">Thrown when driver is not found</exception>
    Task<DriverResponse> DeactivateAsync(Guid id);
}
