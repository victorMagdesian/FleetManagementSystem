using FleetManager.Domain.Entities;

namespace FleetManager.Tests.Unit;

public class DriverTests
{
    [Fact]
    public void Activate_WhenCalled_SetsActiveToTrue()
    {
        // Arrange
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        driver.Deactivate(); // First deactivate
        
        // Act
        driver.Activate();
        
        // Assert
        Assert.True(driver.Active);
    }

    [Fact]
    public void Deactivate_WhenCalled_SetsActiveToFalse()
    {
        // Arrange
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        
        // Act
        driver.Deactivate();
        
        // Assert
        Assert.False(driver.Active);
    }

    [Fact]
    public void IsAvailable_WhenActiveAndNoActiveTrips_ReturnsTrue()
    {
        // Arrange
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        
        // Act
        var isAvailable = driver.IsAvailable();
        
        // Assert
        Assert.True(isAvailable);
    }

    [Fact]
    public void IsAvailable_WhenInactive_ReturnsFalse()
    {
        // Arrange
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        driver.Deactivate();
        
        // Act
        var isAvailable = driver.IsAvailable();
        
        // Assert
        Assert.False(isAvailable);
    }

    [Fact]
    public void IsAvailable_WhenActiveButHasActiveTrip_ReturnsFalse()
    {
        // Arrange
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        var vehicleId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driver.Id, "Route A", DateTime.UtcNow);
        
        // Use reflection to add trip to the collection since it's private set
        var tripsProperty = typeof(Driver).GetProperty("Trips");
        var trips = (ICollection<Trip>)tripsProperty!.GetValue(driver)!;
        trips.Add(trip);
        
        // Act
        var isAvailable = driver.IsAvailable();
        
        // Assert
        Assert.False(isAvailable);
    }

    [Fact]
    public void IsAvailable_WhenActiveAndHasCompletedTrips_ReturnsTrue()
    {
        // Arrange
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        var vehicleId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driver.Id, "Route A", DateTime.UtcNow);
        trip.End(100); // Complete the trip
        
        // Use reflection to add trip to the collection
        var tripsProperty = typeof(Driver).GetProperty("Trips");
        var trips = (ICollection<Trip>)tripsProperty!.GetValue(driver)!;
        trips.Add(trip);
        
        // Act
        var isAvailable = driver.IsAvailable();
        
        // Assert
        Assert.True(isAvailable);
    }

    [Fact]
    public void Constructor_CreatesDriverWithActiveStatusTrue()
    {
        // Act
        var driver = new Driver("John Doe", "DL123456", "555-1234");
        
        // Assert
        Assert.True(driver.Active);
    }
}
