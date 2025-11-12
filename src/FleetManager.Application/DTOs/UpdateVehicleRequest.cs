namespace FleetManager.Application.DTOs;

/// <summary>
/// Request DTO for updating an existing vehicle.
/// </summary>
/// <param name="Model">Vehicle model/make</param>
/// <param name="Year">Manufacturing year</param>
/// <param name="Mileage">Current mileage</param>
public record UpdateVehicleRequest(
    string Model,
    int Year,
    int Mileage
);
