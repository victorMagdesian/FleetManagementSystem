using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using FleetManager.Infrastructure.Jobs;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;

namespace FleetManager.Tests.Unit;

public class MaintenanceCheckJobTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<ILogger<MaintenanceCheckJob>> _mockLogger;
    private readonly Mock<IJobExecutionContext> _mockJobContext;
    private readonly MaintenanceCheckJob _maintenanceCheckJob;

    public MaintenanceCheckJobTests()
    {
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockLogger = new Mock<ILogger<MaintenanceCheckJob>>();
        _mockJobContext = new Mock<IJobExecutionContext>();
        
        _maintenanceCheckJob = new MaintenanceCheckJob(
            _mockVehicleRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Execute_WithVehiclesNeedingMaintenance_LogsAlertsForEachVehicle()
    {
        // Arrange
        var vehicle1 = new Vehicle("ABC1234", "Model 1", 2023, 10000, DateTime.UtcNow.AddDays(-87));
        vehicle1.CalculateNextMaintenanceDate(90);
        
        var vehicle2 = new Vehicle("XYZ5678", "Model 2", 2022, 15000, DateTime.UtcNow.AddDays(-88));
        vehicle2.CalculateNextMaintenanceDate(90);

        var vehicles = new List<Vehicle> { vehicle1, vehicle2 };

        _mockVehicleRepository.Setup(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3))
            .ReturnsAsync(vehicles);

        // Act
        await _maintenanceCheckJob.Execute(_mockJobContext.Object);

        // Assert
        _mockVehicleRepository.Verify(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3), Times.Once);
        
        // Verify alert log for vehicle1
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[ALERTA]") && v.ToString()!.Contains("ABC1234")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
        // Verify alert log for vehicle2
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[ALERTA]") && v.ToString()!.Contains("XYZ5678")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
        // Verify summary log
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MaintenanceCheckJob executado") && v.ToString()!.Contains("2")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WithNoVehiclesNeedingMaintenance_LogsZeroCount()
    {
        // Arrange
        var vehicles = new List<Vehicle>();

        _mockVehicleRepository.Setup(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3))
            .ReturnsAsync(vehicles);

        // Act
        await _maintenanceCheckJob.Execute(_mockJobContext.Object);

        // Assert
        _mockVehicleRepository.Verify(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3), Times.Once);
        
        // Verify no alert logs were generated
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[ALERTA]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
        
        // Verify summary log with zero count
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("MaintenanceCheckJob executado") && v.ToString()!.Contains("0")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WhenRepositoryThrowsException_LogsErrorAndRethrows()
    {
        // Arrange
        var expectedException = new Exception("Database connection failed");
        
        _mockVehicleRepository.Setup(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            _maintenanceCheckJob.Execute(_mockJobContext.Object));
        
        Assert.Equal("Database connection failed", exception.Message);
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error occurred while executing MaintenanceCheckJob")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_ExecutesWithoutErrors()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Model 1", 2023, 10000, DateTime.UtcNow.AddDays(-87));
        vehicle.CalculateNextMaintenanceDate(90);
        
        var vehicles = new List<Vehicle> { vehicle };

        _mockVehicleRepository.Setup(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3))
            .ReturnsAsync(vehicles);

        // Act
        var exception = await Record.ExceptionAsync(() => 
            _maintenanceCheckJob.Execute(_mockJobContext.Object));

        // Assert
        Assert.Null(exception);
        _mockVehicleRepository.Verify(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3), Times.Once);
    }
}
