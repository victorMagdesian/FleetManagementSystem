namespace FleetManager.Application.DTOs;

/// <summary>
/// Response DTO for vehicle data.
/// </summary>
/// <param name="Id">Unique identifier</param>
/// <param name="Plate">License plate number</param>
/// <param name="Model">Vehicle model/make</param>
/// <param name="Year">Manufacturing year</param>
/// <param name="Mileage">Current mileage</param>
/// <param name="LastMaintenanceDate">Date of last maintenance</param>
/// <param name="NextMaintenanceDate">Date of next scheduled maintenance</param>
/// <param name="Status">Current operational status</param>
public record VehicleResponse(
    Guid Id,
    string Plate,
    string Model,
    int Year,
    int Mileage,
    DateTime LastMaintenanceDate,
    DateTime NextMaintenanceDate,
    string Status
);
