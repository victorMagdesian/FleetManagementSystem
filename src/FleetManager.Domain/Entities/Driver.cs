namespace FleetManager.Domain.Entities;

/// <summary>
/// Represents a driver in the fleet.
/// </summary>
public class Driver
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public string Phone { get; private set; }
    public bool Active { get; private set; }
    
    // Navigation property
    public ICollection<Trip> Trips { get; private set; }

    // Private constructor for EF Core
    private Driver()
    {
        Name = string.Empty;
        LicenseNumber = string.Empty;
        Phone = string.Empty;
        Trips = new List<Trip>();
    }

    /// <summary>
    /// Creates a new Driver instance.
    /// </summary>
    /// <param name="name">Driver's full name</param>
    /// <param name="licenseNumber">Driver's license number (must be unique)</param>
    /// <param name="phone">Driver's phone number</param>
    public Driver(string name, string licenseNumber, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number cannot be empty", nameof(licenseNumber));
        
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));

        Id = Guid.NewGuid();
        Name = name;
        LicenseNumber = licenseNumber;
        Phone = phone;
        Active = true; // Drivers are active by default
        Trips = new List<Trip>();
    }

    /// <summary>
    /// Activates the driver, allowing them to be assigned to trips.
    /// </summary>
    public void Activate()
    {
        Active = true;
    }

    /// <summary>
    /// Deactivates the driver, preventing them from being assigned to new trips.
    /// </summary>
    public void Deactivate()
    {
        Active = false;
    }

    /// <summary>
    /// Checks if the driver is available for assignment to a trip.
    /// A driver is available if they are active and not currently on an active trip.
    /// </summary>
    /// <returns>True if the driver is available, false otherwise</returns>
    public bool IsAvailable()
    {
        if (!Active)
            return false;

        // Check if driver has any active trips (trips without an end date)
        return !Trips.Any(t => t.EndDate == null);
    }

    /// <summary>
    /// Updates the driver's information.
    /// </summary>
    /// <param name="name">Updated name</param>
    /// <param name="phone">Updated phone number</param>
    public void UpdateInfo(string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));

        Name = name;
        Phone = phone;
    }
}
