namespace FleetManager.Domain.Enums;

/// <summary>
/// Represents the current operational status of a vehicle in the fleet.
/// </summary>
public enum VehicleStatus
{
    /// <summary>
    /// Vehicle is available for assignment to trips.
    /// </summary>
    Available = 0,

    /// <summary>
    /// Vehicle is currently assigned to an active trip.
    /// </summary>
    InUse = 1,

    /// <summary>
    /// Vehicle is undergoing maintenance and unavailable for trips.
    /// </summary>
    InMaintenance = 2
}
