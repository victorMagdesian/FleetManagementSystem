using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Services;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FleetManager.Tests.Unit;

public class MaintenanceServiceTests
{
    private readonly Mock<IMaintenanceRecordRepository> _mockMaintenanceRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<MaintenanceService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly MaintenanceService _maintenanceService;

    public MaintenanceServiceTests()
    {
        _mockMaintenanceRepository = new Mock<IMaintenanceRecordRepository>();
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<MaintenanceService>>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup default configuration value
        _mockConfiguration.Setup(c => c.GetSection("Maintenance:DefaultIntervalDays").Value)
            .Returns("90");

        _maintenanceService = new MaintenanceService(
            _mockMaintenanceRepository.Object,
            _mockVehicleRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockConfiguration.Object);
    }

    [Fact]
    public async Task CreateAsync_UpdatesVehicleMaintenanceDatesCorrectly()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceDate = new DateTime(2024, 6, 1);
        var request = new CreateMaintenanceRecordRequest(vehicleId, maintenanceDate, "Oil change", 150.00m);

        var vehicle = new Vehicle("ABC1234", "Test Model", 2023, 1000);
        var maintenanceRecord = new MaintenanceRecord(vehicleId, maintenanceDate, "Oil change", 150.00m);
        var expectedResponse = new MaintenanceRecordResponse(
            maintenanceRecord.Id,
            vehicleId,
            maintenanceDate,
            "Oil change",
            150.00m
        );

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockMapper.Setup(m => m.Map<MaintenanceRecord>(request))
            .Returns(maintenanceRecord);
        _mockMapper.Setup(m => m.Map<MaintenanceRecordResponse>(maintenanceRecord))
            .Returns(expectedResponse);
        _mockMaintenanceRepository.Setup(r => r.AddAsync(It.IsAny<MaintenanceRecord>()))
            .Returns(Task.CompletedTask);
        _mockVehicleRepository.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _maintenanceService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(vehicleId, result.VehicleId);
        Assert.Equal(150.00m, result.Cost);
        
        // Verify vehicle maintenance dates were updated
        Assert.Equal(maintenanceDate, vehicle.LastMaintenanceDate);
        Assert.Equal(maintenanceDate.AddDays(90), vehicle.NextMaintenanceDate);
        
        _mockMaintenanceRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceRecord>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UpdatesVehicleStatusToInMaintenance()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var maintenanceDate = DateTime.UtcNow;
        var request = new CreateMaintenanceRecordRequest(vehicleId, maintenanceDate, "Brake replacement", 300.00m);

        var vehicle = new Vehicle("XYZ5678", "Test Model", 2022, 5000);
        var maintenanceRecord = new MaintenanceRecord(vehicleId, maintenanceDate, "Brake replacement", 300.00m);
        var expectedResponse = new MaintenanceRecordResponse(
            maintenanceRecord.Id,
            vehicleId,
            maintenanceDate,
            "Brake replacement",
            300.00m
        );

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync(vehicle);
        _mockMapper.Setup(m => m.Map<MaintenanceRecord>(request))
            .Returns(maintenanceRecord);
        _mockMapper.Setup(m => m.Map<MaintenanceRecordResponse>(maintenanceRecord))
            .Returns(expectedResponse);
        _mockMaintenanceRepository.Setup(r => r.AddAsync(It.IsAny<MaintenanceRecord>()))
            .Returns(Task.CompletedTask);
        _mockVehicleRepository.Setup(r => r.UpdateAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _maintenanceService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        
        // Verify vehicle status was changed to Available after completing maintenance
        Assert.Equal(VehicleStatus.Available, vehicle.Status);
        
        _mockVehicleRepository.Verify(r => r.UpdateAsync(It.Is<Vehicle>(v => v.Status == VehicleStatus.Available)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentVehicle_ThrowsEntityNotFoundException()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var request = new CreateMaintenanceRecordRequest(vehicleId, DateTime.UtcNow, "Test maintenance", 100.00m);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicleId))
            .ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _maintenanceService.CreateAsync(request));
        
        _mockMaintenanceRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceRecord>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>()), Times.Never);
    }
}
