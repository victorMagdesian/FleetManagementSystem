namespace FleetManager.Application.DTOs;

/// <summary>
/// Request DTO for creating a new vehicle.
/// </summary>
/// <param name="Plate">License plate number (must be unique)</param>
/// <param name="Model">Vehicle model/make</param>
/// <param name="Year">Manufacturing year</param>
/// <param name="Mileage">Initial mileage</param>
public record CreateVehicleRequest(
    string Plate,
    string Model,
    int Year,
    int Mileage
);
