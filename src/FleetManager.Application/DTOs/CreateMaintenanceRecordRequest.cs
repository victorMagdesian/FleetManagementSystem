namespace FleetManager.Application.DTOs;

/// <summary>
/// Request DTO for creating a new maintenance record.
/// </summary>
/// <param name="VehicleId">ID of the vehicle being maintained</param>
/// <param name="Date">Date when maintenance was performed</param>
/// <param name="Description">Description of maintenance work</param>
/// <param name="Cost">Cost of maintenance</param>
public record CreateMaintenanceRecordRequest(
    Guid VehicleId,
    DateTime Date,
    string Description,
    decimal Cost
);
