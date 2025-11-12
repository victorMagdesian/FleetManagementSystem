namespace FleetManager.Application.DTOs;

/// <summary>
/// Request DTO for starting a new trip.
/// </summary>
/// <param name="VehicleId">ID of the vehicle for this trip</param>
/// <param name="DriverId">ID of the driver for this trip</param>
/// <param name="Route">Route or destination description</param>
public record StartTripRequest(
    Guid VehicleId,
    Guid DriverId,
    string Route
);
