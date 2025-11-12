using FleetManager.Domain.Entities;

namespace FleetManager.Tests.Unit;

public class TripTests
{
    [Fact]
    public void End_WhenTripIsActive_SetsEndDateAndDistance()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        
        // Act
        trip.End(150);
        
        // Assert
        Assert.NotNull(trip.EndDate);
        Assert.Equal(150, trip.Distance);
    }

    [Fact]
    public void End_WhenTripAlreadyEnded_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        trip.End(150);
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => trip.End(100));
        Assert.Contains("already been ended", exception.Message);
    }

    [Fact]
    public void End_WithNegativeDistance_ThrowsArgumentException()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => trip.End(-50));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void IsActive_WhenTripNotEnded_ReturnsTrue()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        
        // Act
        var isActive = trip.IsActive();
        
        // Assert
        Assert.True(isActive);
    }

    [Fact]
    public void IsActive_WhenTripEnded_ReturnsFalse()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        trip.End(150);
        
        // Act
        var isActive = trip.IsActive();
        
        // Assert
        Assert.False(isActive);
    }

    [Fact]
    public void Constructor_CreatesActiveTripWithZeroDistance()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        
        // Act
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        
        // Assert
        Assert.True(trip.IsActive());
        Assert.Equal(0, trip.Distance);
        Assert.Null(trip.EndDate);
    }
}
