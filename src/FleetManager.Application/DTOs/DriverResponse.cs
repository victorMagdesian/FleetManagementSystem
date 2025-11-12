namespace FleetManager.Application.DTOs;

/// <summary>
/// Response DTO for driver data.
/// </summary>
/// <param name="Id">Unique identifier</param>
/// <param name="Name">Driver's full name</param>
/// <param name="LicenseNumber">Driver's license number</param>
/// <param name="Phone">Driver's phone number</param>
/// <param name="Active">Whether the driver is active</param>
public record DriverResponse(
    Guid Id,
    string Name,
    string LicenseNumber,
    string Phone,
    bool Active
);
