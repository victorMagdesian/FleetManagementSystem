using FleetManager.Domain.Enums;

namespace FleetManager.Domain.Entities;

/// <summary>
/// Represents a vehicle in the fleet with maintenance tracking and status management.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Unique identifier for the vehicle.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// License plate number (unique identifier).
    /// </summary>
    public string Plate { get; private set; }

    /// <summary>
    /// Vehicle model/make.
    /// </summary>
    public string Model { get; private set; }

    /// <summary>
    /// Manufacturing year.
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// Current mileage/odometer reading.
    /// </summary>
    public int Mileage { get; private set; }

    /// <summary>
    /// Date of the last completed maintenance.
    /// </summary>
    public DateTime LastMaintenanceDate { get; private set; }

    /// <summary>
    /// Calculated date for the next scheduled maintenance.
    /// </summary>
    public DateTime NextMaintenanceDate { get; private set; }

    /// <summary>
    /// Current operational status of the vehicle.
    /// </summary>
    public VehicleStatus Status { get; private set; }

    /// <summary>
    /// Navigation property for maintenance records associated with this vehicle.
    /// </summary>
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; private set; }

    /// <summary>
    /// Navigation property for trips associated with this vehicle.
    /// </summary>
    public ICollection<Trip> Trips { get; private set; }

    // Private constructor for EF Core
    private Vehicle()
    {
        MaintenanceRecords = new List<MaintenanceRecord>();
        Trips = new List<Trip>();
    }

    /// <summary>
    /// Creates a new vehicle instance.
    /// </summary>
    /// <param name="plate">License plate number</param>
    /// <param name="model">Vehicle model</param>
    /// <param name="year">Manufacturing year</param>
    /// <param name="mileage">Initial mileage</param>
    /// <param name="lastMaintenanceDate">Date of last maintenance (optional, defaults to creation date)</param>
    public Vehicle(string plate, string model, int year, int mileage, DateTime? lastMaintenanceDate = null)
    {
        if (string.IsNullOrWhiteSpace(plate))
            throw new ArgumentException("Plate cannot be empty", nameof(plate));
        
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty", nameof(model));
        
        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new ArgumentException("Invalid year", nameof(year));
        
        if (mileage < 0)
            throw new ArgumentException("Mileage cannot be negative", nameof(mileage));

        Id = Guid.NewGuid();
        Plate = plate;
        Model = model;
        Year = year;
        Mileage = mileage;
        LastMaintenanceDate = lastMaintenanceDate ?? DateTime.UtcNow;
        NextMaintenanceDate = LastMaintenanceDate;
        Status = VehicleStatus.Available;
        MaintenanceRecords = new List<MaintenanceRecord>();
        Trips = new List<Trip>();
    }

    /// <summary>
    /// Updates the vehicle's mileage.
    /// </summary>
    /// <param name="newMileage">New mileage value</param>
    /// <exception cref="ArgumentException">Thrown when new mileage is less than current mileage</exception>
    public void UpdateMileage(int newMileage)
    {
        if (newMileage < Mileage)
            throw new ArgumentException("New mileage cannot be less than current mileage", nameof(newMileage));

        Mileage = newMileage;
    }

    /// <summary>
    /// Marks the vehicle as in use when a trip starts.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is not available</exception>
    public void StartTrip()
    {
        if (Status != VehicleStatus.Available)
            throw new InvalidOperationException($"Vehicle cannot start trip. Current status: {Status}");

        Status = VehicleStatus.InUse;
    }

    /// <summary>
    /// Marks the vehicle as available when a trip ends and updates mileage.
    /// </summary>
    /// <param name="distanceTraveled">Distance traveled during the trip</param>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is not in use</exception>
    /// <exception cref="ArgumentException">Thrown when distance is negative</exception>
    public void EndTrip(int distanceTraveled)
    {
        if (Status != VehicleStatus.InUse)
            throw new InvalidOperationException($"Vehicle is not currently in use. Current status: {Status}");

        if (distanceTraveled < 0)
            throw new ArgumentException("Distance traveled cannot be negative", nameof(distanceTraveled));

        Mileage += distanceTraveled;
        Status = VehicleStatus.Available;
    }

    /// <summary>
    /// Marks the vehicle as in maintenance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is currently in use</exception>
    public void StartMaintenance()
    {
        if (Status == VehicleStatus.InUse)
            throw new InvalidOperationException("Cannot start maintenance while vehicle is in use");

        Status = VehicleStatus.InMaintenance;
    }

    /// <summary>
    /// Completes maintenance, updates maintenance dates, and marks vehicle as available.
    /// </summary>
    /// <param name="maintenanceDate">Date when maintenance was completed</param>
    /// <param name="intervalDays">Number of days until next maintenance (optional, defaults to 90)</param>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is not in maintenance</exception>
    public void CompleteMaintenance(DateTime maintenanceDate, int intervalDays = 90)
    {
        if (Status != VehicleStatus.InMaintenance)
            throw new InvalidOperationException($"Vehicle is not in maintenance. Current status: {Status}");

        if (intervalDays <= 0)
            throw new ArgumentException("Interval days must be positive", nameof(intervalDays));

        LastMaintenanceDate = maintenanceDate;
        CalculateNextMaintenanceDate(intervalDays);
        Status = VehicleStatus.Available;
    }

    /// <summary>
    /// Calculates and updates the next maintenance date based on the last maintenance date.
    /// </summary>
    /// <param name="daysInterval">Number of days to add to last maintenance date</param>
    /// <exception cref="ArgumentException">Thrown when interval is not positive</exception>
    public void CalculateNextMaintenanceDate(int daysInterval)
    {
        if (daysInterval <= 0)
            throw new ArgumentException("Days interval must be positive", nameof(daysInterval));

        NextMaintenanceDate = LastMaintenanceDate.AddDays(daysInterval);
    }

    /// <summary>
    /// Checks if maintenance is due within the specified threshold.
    /// </summary>
    /// <param name="daysThreshold">Number of days to check ahead</param>
    /// <returns>True if maintenance is due within the threshold, false otherwise</returns>
    public bool IsMaintenanceDue(int daysThreshold)
    {
        if (daysThreshold < 0)
            throw new ArgumentException("Days threshold cannot be negative", nameof(daysThreshold));

        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return NextMaintenanceDate <= thresholdDate;
    }
}
