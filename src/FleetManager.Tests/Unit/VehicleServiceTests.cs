using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Application.Services;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace FleetManager.Tests.Unit;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<VehicleService>> _mockLogger;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly VehicleService _vehicleService;

    public VehicleServiceTests()
    {
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<VehicleService>>();
        _mockCacheService = new Mock<ICacheService>();
        
        _vehicleService = new VehicleService(
            _mockVehicleRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesVehicleSuccessfully()
    {
        // Arrange
        var request = new CreateVehicleRequest("ABC1234", "Test Model", 2023, 1000);

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        var expectedResponse = new VehicleResponse(
            vehicle.Id,
            "ABC1234",
            "Test Model",
            2023,
            1000,
            vehicle.LastMaintenanceDate,
            vehicle.NextMaintenanceDate,
            VehicleStatus.Available.ToString()
        );

        _mockVehicleRepository.Setup(r => r.GetByPlateAsync(request.Plate))
            .ReturnsAsync((Vehicle?)null);
        _mockMapper.Setup(m => m.Map<Vehicle>(request))
            .Returns(vehicle);
        _mockMapper.Setup(m => m.Map<VehicleResponse>(vehicle))
            .Returns(expectedResponse);
        _mockVehicleRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);
        _mockCacheService.Setup(c => c.RemoveAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _vehicleService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC1234", result.Plate);
        _mockVehicleRepository.Verify(r => r.GetByPlateAsync(request.Plate), Times.Once);
        _mockVehicleRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
        _mockCacheService.Verify(c => c.RemoveAsync("vehicles:available"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicatePlate_ThrowsDuplicateEntityException()
    {
        // Arrange
        var request = new CreateVehicleRequest("ABC1234", "Test Model", 2023, 1000);

        var existingVehicle = new Vehicle("ABC1234", "Existing Model", 2022, 5000);

        _mockVehicleRepository.Setup(r => r.GetByPlateAsync(request.Plate))
            .ReturnsAsync(existingVehicle);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => 
            _vehicleService.CreateAsync(request));
        
        _mockVehicleRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
    }

    [Fact]
    public async Task GetAvailableAsync_ReturnsFilteredVehicles()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Vehicle("ABC1234", "Model 1", 2023, 1000),
            new Vehicle("XYZ5678", "Model 2", 2022, 2000)
        };

        var expectedResponses = new List<VehicleResponse>
        {
            new VehicleResponse(vehicles[0].Id, "ABC1234", "Model 1", 2023, 1000, vehicles[0].LastMaintenanceDate, vehicles[0].NextMaintenanceDate, VehicleStatus.Available.ToString()),
            new VehicleResponse(vehicles[1].Id, "XYZ5678", "Model 2", 2022, 2000, vehicles[1].LastMaintenanceDate, vehicles[1].NextMaintenanceDate, VehicleStatus.Available.ToString())
        };

        _mockCacheService.Setup(c => c.GetAsync<IEnumerable<VehicleResponse>>("vehicles:available"))
            .ReturnsAsync((IEnumerable<VehicleResponse>?)null);
        _mockVehicleRepository.Setup(r => r.GetAvailableAsync())
            .ReturnsAsync(vehicles);
        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleResponse>>(vehicles))
            .Returns(expectedResponses);
        _mockCacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<VehicleResponse>>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _vehicleService.GetAvailableAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockVehicleRepository.Verify(r => r.GetAvailableAsync(), Times.Once);
        _mockCacheService.Verify(c => c.SetAsync("vehicles:available", It.IsAny<IEnumerable<VehicleResponse>>(), 5), Times.Once);
    }

    [Fact]
    public async Task GetUpcomingMaintenanceAsync_WithValidThreshold_ReturnsFilteredVehicles()
    {
        // Arrange
        var daysThreshold = 7;
        var vehicles = new List<Vehicle>
        {
            new Vehicle("ABC1234", "Model 1", 2023, 1000, DateTime.UtcNow.AddDays(-80))
        };
        vehicles[0].CalculateNextMaintenanceDate(90);

        var expectedResponses = new List<VehicleResponse>
        {
            new VehicleResponse(vehicles[0].Id, "ABC1234", "Model 1", 2023, 1000, vehicles[0].LastMaintenanceDate, vehicles[0].NextMaintenanceDate, VehicleStatus.Available.ToString())
        };

        _mockVehicleRepository.Setup(r => r.GetVehiclesWithUpcomingMaintenanceAsync(daysThreshold))
            .ReturnsAsync(vehicles);
        _mockMapper.Setup(m => m.Map<IEnumerable<VehicleResponse>>(vehicles))
            .Returns(expectedResponses);

        // Act
        var result = await _vehicleService.GetUpcomingMaintenanceAsync(daysThreshold);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockVehicleRepository.Verify(r => r.GetVehiclesWithUpcomingMaintenanceAsync(daysThreshold), Times.Once);
    }
}
