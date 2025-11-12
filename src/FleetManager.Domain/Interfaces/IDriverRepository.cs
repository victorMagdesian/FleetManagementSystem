using FleetManager.Domain.Entities;

namespace FleetManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Driver entity operations.
/// Defines data access methods for driver management.
/// </summary>
public interface IDriverRepository
{
    /// <summary>
    /// Retrieves a driver by their unique identifier.
    /// </summary>
    /// <param name="id">The driver's unique identifier</param>
    /// <returns>The driver if found, null otherwise</returns>
    Task<Driver?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all drivers in the system.
    /// </summary>
    /// <returns>Collection of all drivers</returns>
    Task<IEnumerable<Driver>> GetAllAsync();

    /// <summary>
    /// Retrieves all active and available drivers.
    /// </summary>
    /// <returns>Collection of available drivers</returns>
    Task<IEnumerable<Driver>> GetAvailableAsync();

    /// <summary>
    /// Retrieves a driver by their license number.
    /// </summary>
    /// <param name="licenseNumber">The driver's license number</param>
    /// <returns>The driver if found, null otherwise</returns>
    Task<Driver?> GetByLicenseNumberAsync(string licenseNumber);

    /// <summary>
    /// Adds a new driver to the repository.
    /// </summary>
    /// <param name="driver">The driver to add</param>
    Task AddAsync(Driver driver);

    /// <summary>
    /// Updates an existing driver in the repository.
    /// </summary>
    /// <param name="driver">The driver to update</param>
    Task UpdateAsync(Driver driver);

    /// <summary>
    /// Deletes a driver from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the driver to delete</param>
    Task DeleteAsync(Guid id);
}
