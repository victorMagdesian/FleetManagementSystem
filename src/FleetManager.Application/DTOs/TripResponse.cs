namespace FleetManager.Application.DTOs;

/// <summary>
/// Response DTO for trip data.
/// </summary>
/// <param name="Id">Unique identifier</param>
/// <param name="VehicleId">ID of the associated vehicle</param>
/// <param name="DriverId">ID of the associated driver</param>
/// <param name="Route">Route or destination description</param>
/// <param name="StartDate">Date and time when trip started</param>
/// <param name="EndDate">Date and time when trip ended (null if active)</param>
/// <param name="Distance">Distance traveled in kilometers</param>
public record TripResponse(
    Guid Id,
    Guid VehicleId,
    Guid DriverId,
    string Route,
    DateTime StartDate,
    DateTime? EndDate,
    int Distance
);
