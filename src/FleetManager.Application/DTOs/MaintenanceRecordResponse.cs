namespace FleetManager.Application.DTOs;

/// <summary>
/// Response DTO for maintenance record data.
/// </summary>
/// <param name="Id">Unique identifier</param>
/// <param name="VehicleId">ID of the associated vehicle</param>
/// <param name="Date">Date when maintenance was performed</param>
/// <param name="Description">Description of maintenance work</param>
/// <param name="Cost">Cost of maintenance</param>
public record MaintenanceRecordResponse(
    Guid Id,
    Guid VehicleId,
    DateTime Date,
    string Description,
    decimal Cost
);
