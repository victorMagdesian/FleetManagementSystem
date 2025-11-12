using FleetManager.Application.DTOs;

namespace FleetManager.Application.Interfaces;

/// <summary>
/// Service interface for maintenance record management operations.
/// </summary>
public interface IMaintenanceService
{
    /// <summary>
    /// Creates a new maintenance record and updates the associated vehicle.
    /// Updates vehicle's LastMaintenanceDate, calculates NextMaintenanceDate,
    /// and sets vehicle status to InMaintenance.
    /// </summary>
    /// <param name="request">The maintenance record creation request</param>
    /// <returns>The created maintenance record response</returns>
    Task<MaintenanceRecordResponse> CreateAsync(CreateMaintenanceRecordRequest request);

    /// <summary>
    /// Retrieves all maintenance records for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The vehicle's unique identifier</param>
    /// <returns>Collection of maintenance records for the vehicle</returns>
    Task<IEnumerable<MaintenanceRecordResponse>> GetByVehicleIdAsync(Guid vehicleId);
}
