using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Services;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using InvalidOperationException = FleetManager.Application.Exceptions.InvalidOperationException;

namespace FleetManager.Tests.Unit;

public class TripServiceTests
{
    private readonly Mock<ITripRepository> _mockTripRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IDriverRepository> _mockDriverRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<TripService>> _mockLogger;
    private readonly TripService _tripService;

    public TripServiceTests()
    {
        _mockTripRepository = new Mock<ITripRepository>();
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockDriverRepository = new Mock<IDriverRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<TripService>>();

        _tripService = new TripService(
            _mockTripRepository.Object,
            _mockVehicleRepository.Object,
            _mockDriverRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task StartTripAsync_ValidatesVehicleAvailability()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var request = new StartTripRequest(vehicleId, driverId, "Route A to B");

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        vehicle.StartTrip(); // Make vehicle unavailable

        var driver = new Driver("John Doe", "DL123456", "+1234567890");

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockDriverRepository.Setup(r => r.GetByIdAsync(driverId))
            .ReturnsAsync(driver);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _tripService.StartTripAsync(request));
        
        Assert.Contains("not available", exception.Message);
        _mockTripRepository.Verify(r => r.AddAsync(It.IsAny<Trip>()), Times.Never);
    }

    [Fact]
    public async Task StartTripAsync_ValidatesDriverAvailability()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var request = new StartTripRequest(vehicleId, driverId, "Route A to B");

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        var driver = new Driver("John Doe", "DL123456", "+1234567890");
        driver.Deactivate(); // Make driver inactive

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockDriverRepository.Setup(r => r.GetByIdAsync(driverId))
            .ReturnsAsync(driver);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _tripService.StartTripAsync(request));
        
        Assert.Contains("not active", exception.Message);
        _mockTripRepository.Verify(r => r.AddAsync(It.IsAny<Trip>()), Times.Never);
    }

    [Fact]
    public async Task StartTripAsync_UpdatesVehicleStatusToInUse()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var request = new StartTripRequest(vehicleId, driverId, "Route A to B");

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        var driver = new Driver("John Doe", "DL123456", "+1234567890");
        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        var expectedResponse = new TripResponse(
            trip.Id,
            vehicleId,
            driverId,
            "Route A to B",
            trip.StartDate,
            trip.EndDate,
            trip.Distance
        );

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockDriverRepository.Setup(r => r.GetByIdAsync(driverId))
            .ReturnsAsync(driver);
        _mockTripRepository.Setup(r => r.GetActiveTripsAsync())
            .ReturnsAsync(new List<Trip>());
        _mockMapper.Setup(m => m.Map<TripResponse>(It.IsAny<Trip>()))
            .Returns(expectedResponse);
        _mockTripRepository.Setup(r => r.AddAsync(It.IsAny<Trip>()))
            .Returns(Task.CompletedTask);
        _mockVehicleRepository.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tripService.StartTripAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VehicleStatus.InUse, vehicle.Status);
        _mockTripRepository.Verify(r => r.AddAsync(It.IsAny<Trip>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.UpdateAsync(It.Is<Vehicle>(v => v.Status == VehicleStatus.InUse)), Times.Once);
    }

    [Fact]
    public async Task EndTripAsync_UpdatesVehicleMileageAndStatus()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var initialMileage = 1000;
        var tripDistance = 150;

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, initialMileage);
        vehicle.StartTrip();

        var trip = new Trip(vehicleId, driverId, "Route A to B", DateTime.UtcNow);
        var request = new EndTripRequest(tripDistance);

        var expectedResponse = new TripResponse(
            trip.Id,
            vehicleId,
            driverId,
            "Route A to B",
            trip.StartDate,
            trip.EndDate,
            tripDistance
        );

        _mockTripRepository.Setup(r => r.GetByIdAsync(tripId))
            .ReturnsAsync(trip);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockMapper.Setup(m => m.Map<TripResponse>(trip))
            .Returns(expectedResponse);
        _mockTripRepository.Setup(r => r.UpdateAsync(It.IsAny<Trip>()))
            .Returns(Task.CompletedTask);
        _mockVehicleRepository.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tripService.EndTripAsync(tripId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
        Assert.Equal(initialMileage + tripDistance, vehicle.Mileage);
        _mockTripRepository.Verify(r => r.UpdateAsync(It.IsAny<Trip>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.UpdateAsync(It.Is<Vehicle>(v => 
            v.Status == VehicleStatus.Available && v.Mileage == initialMileage + tripDistance)), Times.Once);
    }

    [Fact]
    public async Task StartTripAsync_WithDriverOnActiveTrip_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();
        var request = new StartTripRequest(vehicleId, driverId, "Route A to B");

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        var driver = new Driver("John Doe", "DL123456", "+1234567890");
        var activeTrip = new Trip(Guid.NewGuid(), driverId, "Another Route", DateTime.UtcNow);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockDriverRepository.Setup(r => r.GetByIdAsync(driverId))
            .ReturnsAsync(driver);
        _mockTripRepository.Setup(r => r.GetActiveTripsAsync())
            .ReturnsAsync(new List<Trip> { activeTrip });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _tripService.StartTripAsync(request));
        
        Assert.Contains("already on an active trip", exception.Message);
        _mockTripRepository.Verify(r => r.AddAsync(It.IsAny<Trip>()), Times.Never);
    }

    [Fact]
    public async Task EndTripAsync_WithNonExistentTrip_ThrowsEntityNotFoundException()
    {
        // Arrange
        var tripId = Guid.NewGuid();
        var request = new EndTripRequest(100);

        _mockTripRepository.Setup(r => r.GetByIdAsync(tripId))
            .ReturnsAsync((Trip?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _tripService.EndTripAsync(tripId, request));
        
        _mockTripRepository.Verify(r => r.UpdateAsync(It.IsAny<Trip>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Never);
    }
}
