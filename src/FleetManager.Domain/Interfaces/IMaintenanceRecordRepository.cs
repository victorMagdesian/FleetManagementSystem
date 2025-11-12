using FleetManager.Domain.Entities;

namespace FleetManager.Domain.Interfaces;

/// <summary>
/// Repository interface for MaintenanceRecord entity operations.
/// Defines data access methods for maintenance record management.
/// </summary>
public interface IMaintenanceRecordRepository
{
    /// <summary>
    /// Retrieves a maintenance record by its unique identifier.
    /// </summary>
    /// <param name="id">The maintenance record's unique identifier</param>
    /// <returns>The maintenance record if found, null otherwise</returns>
    Task<MaintenanceRecord?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all maintenance records for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The vehicle's unique identifier</param>
    /// <returns>Collection of maintenance records for the vehicle</returns>
    Task<IEnumerable<MaintenanceRecord>> GetByVehicleIdAsync(Guid vehicleId);

    /// <summary>
    /// Adds a new maintenance record to the repository.
    /// </summary>
    /// <param name="record">The maintenance record to add</param>
    Task AddAsync(MaintenanceRecord record);
}
