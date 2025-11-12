using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;

namespace FleetManager.Tests.Unit;

public class VehicleTests
{
    [Fact]
    public void UpdateMileage_WithValidMileage_UpdatesSuccessfully()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act
        vehicle.UpdateMileage(1500);
        
        // Assert
        Assert.Equal(1500, vehicle.Mileage);
    }

    [Fact]
    public void UpdateMileage_WithLowerMileage_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => vehicle.UpdateMileage(500));
        Assert.Contains("cannot be less than current mileage", exception.Message);
    }

    [Fact]
    public void StartTrip_WhenAvailable_ChangesStatusToInUse()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act
        vehicle.StartTrip();
        
        // Assert
        Assert.Equal(VehicleStatus.InUse, vehicle.Status);
    }

    [Fact]
    public void StartTrip_WhenInMaintenance_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        vehicle.StartMaintenance();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => vehicle.StartTrip());
        Assert.Contains("cannot start trip", exception.Message);
    }

    [Fact]
    public void EndTrip_WhenInUse_ChangesStatusToAvailableAndUpdatesMileage()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        vehicle.StartTrip();
        
        // Act
        vehicle.EndTrip(150);
        
        // Assert
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
        Assert.Equal(1150, vehicle.Mileage);
    }

    [Fact]
    public void EndTrip_WhenNotInUse_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => vehicle.EndTrip(150));
        Assert.Contains("not currently in use", exception.Message);
    }

    [Fact]
    public void EndTrip_WithNegativeDistance_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        vehicle.StartTrip();
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => vehicle.EndTrip(-50));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void StartMaintenance_WhenAvailable_ChangesStatusToInMaintenance()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act
        vehicle.StartMaintenance();
        
        // Assert
        Assert.Equal(VehicleStatus.InMaintenance, vehicle.Status);
    }

    [Fact]
    public void StartMaintenance_WhenInUse_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        vehicle.StartTrip();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => vehicle.StartMaintenance());
        Assert.Contains("Cannot start maintenance while vehicle is in use", exception.Message);
    }

    [Fact]
    public void CompleteMaintenance_WhenInMaintenance_UpdatesDatesAndChangesStatusToAvailable()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000, new DateTime(2024, 1, 1));
        vehicle.StartMaintenance();
        var maintenanceDate = new DateTime(2024, 6, 1);
        
        // Act
        vehicle.CompleteMaintenance(maintenanceDate, 90);
        
        // Assert
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
        Assert.Equal(maintenanceDate, vehicle.LastMaintenanceDate);
        Assert.Equal(new DateTime(2024, 8, 30), vehicle.NextMaintenanceDate);
    }

    [Fact]
    public void CompleteMaintenance_WhenNotInMaintenance_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            vehicle.CompleteMaintenance(DateTime.UtcNow));
        Assert.Contains("not in maintenance", exception.Message);
    }

    [Fact]
    public void CalculateNextMaintenanceDate_WithValidInterval_UpdatesNextMaintenanceDate()
    {
        // Arrange
        var lastMaintenance = new DateTime(2024, 1, 1);
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000, lastMaintenance);
        
        // Act
        vehicle.CalculateNextMaintenanceDate(90);
        
        // Assert
        Assert.Equal(new DateTime(2024, 3, 31), vehicle.NextMaintenanceDate);
    }

    [Fact]
    public void CalculateNextMaintenanceDate_WithZeroInterval_ThrowsArgumentException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            vehicle.CalculateNextMaintenanceDate(0));
        Assert.Contains("must be positive", exception.Message);
    }

    [Fact]
    public void IsMaintenanceDue_WhenNextMaintenanceIsWithinThreshold_ReturnsTrue()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000, DateTime.UtcNow);
        vehicle.CalculateNextMaintenanceDate(2); // Next maintenance in 2 days
        
        // Act
        var isDue = vehicle.IsMaintenanceDue(3); // Check within 3 days
        
        // Assert
        Assert.True(isDue);
    }

    [Fact]
    public void IsMaintenanceDue_WhenNextMaintenanceIsBeyondThreshold_ReturnsFalse()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000, DateTime.UtcNow);
        vehicle.CalculateNextMaintenanceDate(10); // Next maintenance in 10 days
        
        // Act
        var isDue = vehicle.IsMaintenanceDue(3); // Check within 3 days
        
        // Assert
        Assert.False(isDue);
    }

    [Fact]
    public void IsMaintenanceDue_WhenNextMaintenanceIsToday_ReturnsTrue()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000, DateTime.UtcNow);
        vehicle.CalculateNextMaintenanceDate(1); // Next maintenance tomorrow
        
        // Act
        var isDue = vehicle.IsMaintenanceDue(1); // Check within 1 day
        
        // Assert
        Assert.True(isDue);
    }

    [Fact]
    public void IsMaintenanceDue_WhenNextMaintenanceIsPast_ReturnsTrue()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-10);
        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000, pastDate);
        vehicle.CalculateNextMaintenanceDate(5); // Next maintenance was 5 days ago
        
        // Act
        var isDue = vehicle.IsMaintenanceDue(3);
        
        // Assert
        Assert.True(isDue);
    }
}
