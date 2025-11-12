namespace FleetManager.Application.DTOs;

/// <summary>
/// Request DTO for creating a new driver.
/// </summary>
/// <param name="Name">Driver's full name</param>
/// <param name="LicenseNumber">Driver's license number (must be unique)</param>
/// <param name="Phone">Driver's phone number</param>
public record CreateDriverRequest(
    string Name,
    string LicenseNumber,
    string Phone
);
