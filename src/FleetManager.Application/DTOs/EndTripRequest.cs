namespace FleetManager.Application.DTOs;

/// <summary>
/// Request DTO for ending a trip.
/// </summary>
/// <param name="Distance">Distance traveled during the trip in kilometers</param>
public record EndTripRequest(
    int Distance
);
