using System.Net;
using System.Net.Http.Json;
using FleetManager.Application.DTOs;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetManager.Tests.Integration;

public class MaintenanceControllerTests : IClassFixture<FleetManagerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly FleetManagerWebApplicationFactory _factory;

    public MaintenanceControllerTests(FleetManagerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidMaintenanceRecord_CreatesRecordAndUpdatesVehicle()
    {
        // Arrange
        await ClearDatabase();
        
        // Create a vehicle
        var vehicleRequest = new CreateVehicleRequest("MNT1234", "Chevrolet Malibu", 2023, 2000);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle);

        var maintenanceRequest = new CreateMaintenanceRecordRequest(
            vehicle.Id,
            DateTime.UtcNow,
            "Oil change and tire rotation",
            150.00m);

        // Act
        var response = await _client.PostAsJsonAsync("/api/maintenance", maintenanceRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var maintenanceRecord = await response.Content.ReadFromJsonAsync<MaintenanceRecordResponse>();
        Assert.NotNull(maintenanceRecord);
        Assert.Equal(vehicle.Id, maintenanceRecord.VehicleId);
        Assert.Equal("Oil change and tire rotation", maintenanceRecord.Description);
        Assert.Equal(150.00m, maintenanceRecord.Cost);

        // Verify vehicle maintenance dates are updated
        var vehicleCheckResponse = await _client.GetAsync($"/api/vehicles/{vehicle.Id}");
        var updatedVehicle = await vehicleCheckResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(updatedVehicle);
        // After completing maintenance, vehicle should be Available
        Assert.Equal("Available", updatedVehicle.Status);
        Assert.True(updatedVehicle.NextMaintenanceDate > updatedVehicle.LastMaintenanceDate);
    }

    [Fact]
    public async Task GetUpcoming_ReturnsVehiclesWithUpcomingMaintenance()
    {
        // Arrange
        await ClearDatabase();
        
        // Create vehicles with different maintenance dates
        var vehicle1Request = new CreateVehicleRequest("MNT5678", "Toyota RAV4", 2022, 5000);
        var vehicle1Response = await _client.PostAsJsonAsync("/api/vehicles", vehicle1Request);
        var vehicle1 = await vehicle1Response.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle1);

        var vehicle2Request = new CreateVehicleRequest("MNT9012", "Honda CR-V", 2021, 10000);
        var vehicle2Response = await _client.PostAsJsonAsync("/api/vehicles", vehicle2Request);
        var vehicle2 = await vehicle2Response.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle2);

        // Create maintenance record for vehicle1 (will have upcoming maintenance)
        var maintenance1Request = new CreateMaintenanceRecordRequest(
            vehicle1.Id,
            DateTime.UtcNow.AddDays(-88), // 88 days ago, so next maintenance is in 2 days (90-day interval)
            "Regular maintenance",
            200.00m);
        await _client.PostAsJsonAsync("/api/maintenance", maintenance1Request);

        // Create maintenance record for vehicle2 (maintenance not upcoming)
        var maintenance2Request = new CreateMaintenanceRecordRequest(
            vehicle2.Id,
            DateTime.UtcNow.AddDays(-30), // 30 days ago, so next maintenance is in 60 days
            "Regular maintenance",
            250.00m);
        await _client.PostAsJsonAsync("/api/maintenance", maintenance2Request);

        // Act
        var response = await _client.GetAsync("/api/maintenance/upcoming?daysThreshold=7");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var vehicles = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        Assert.NotNull(vehicles);
        Assert.Single(vehicles);
        Assert.Equal(vehicle1.Id, vehicles[0].Id);
    }

    [Fact]
    public async Task Create_InvalidVehicleId_ReturnsNotFound()
    {
        // Arrange
        await ClearDatabase();
        
        var maintenanceRequest = new CreateMaintenanceRecordRequest(
            Guid.NewGuid(),
            DateTime.UtcNow,
            "Invalid maintenance",
            100.00m);

        // Act
        var response = await _client.PostAsJsonAsync("/api/maintenance", maintenanceRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByVehicleId_ReturnsMaintenanceRecordsForVehicle()
    {
        // Arrange
        await ClearDatabase();
        
        // Create a vehicle
        var vehicleRequest = new CreateVehicleRequest("MNT3456", "Nissan Rogue", 2020, 15000);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle);

        // Create multiple maintenance records
        var maintenance1 = new CreateMaintenanceRecordRequest(
            vehicle.Id,
            DateTime.UtcNow.AddDays(-60),
            "First maintenance",
            150.00m);
        await _client.PostAsJsonAsync("/api/maintenance", maintenance1);

        var maintenance2 = new CreateMaintenanceRecordRequest(
            vehicle.Id,
            DateTime.UtcNow.AddDays(-30),
            "Second maintenance",
            200.00m);
        await _client.PostAsJsonAsync("/api/maintenance", maintenance2);

        // Act
        var response = await _client.GetAsync($"/api/maintenance/vehicle/{vehicle.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var records = await response.Content.ReadFromJsonAsync<List<MaintenanceRecordResponse>>();
        Assert.NotNull(records);
        Assert.Equal(2, records.Count);
    }

    private async Task ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FleetManagerDbContext>();
        
        // Load all entities first to avoid concurrency issues
        var trips = await dbContext.Trips.ToListAsync();
        var maintenanceRecords = await dbContext.MaintenanceRecords.ToListAsync();
        var vehicles = await dbContext.Vehicles.ToListAsync();
        var drivers = await dbContext.Drivers.ToListAsync();
        
        dbContext.Trips.RemoveRange(trips);
        dbContext.MaintenanceRecords.RemoveRange(maintenanceRecords);
        dbContext.Vehicles.RemoveRange(vehicles);
        dbContext.Drivers.RemoveRange(drivers);
        
        await dbContext.SaveChangesAsync();
    }
}
