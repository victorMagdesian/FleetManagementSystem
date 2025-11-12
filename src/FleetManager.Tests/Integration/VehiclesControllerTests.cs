using System.Net;
using System.Net.Http.Json;
using FleetManager.Application.DTOs;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetManager.Tests.Integration;

public class VehiclesControllerTests : IClassFixture<FleetManagerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly FleetManagerWebApplicationFactory _factory;

    public VehiclesControllerTests(FleetManagerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidVehicle_ReturnsCreatedWithVehicle()
    {
        // Arrange
        var request = new CreateVehicleRequest("ABC1234", "Toyota Corolla", 2023, 0);

        // Act
        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle);
        Assert.Equal("ABC1234", vehicle.Plate);
        Assert.Equal("Toyota Corolla", vehicle.Model);
        Assert.Equal(2023, vehicle.Year);
        Assert.Equal(0, vehicle.Mileage);
    }

    [Fact]
    public async Task GetAll_ReturnsAllVehicles()
    {
        // Arrange
        await ClearDatabase();
        var vehicle1 = new CreateVehicleRequest("XYZ5678", "Honda Civic", 2022, 5000);
        var vehicle2 = new CreateVehicleRequest("DEF9012", "Ford Focus", 2021, 10000);
        await _client.PostAsJsonAsync("/api/vehicles", vehicle1);
        await _client.PostAsJsonAsync("/api/vehicles", vehicle2);

        // Act
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var vehicles = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        Assert.NotNull(vehicles);
        Assert.Equal(2, vehicles.Count);
    }

    [Fact]
    public async Task GetById_NonExistentVehicle_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/vehicles/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ExistingVehicle_ReturnsUpdatedVehicle()
    {
        // Arrange
        await ClearDatabase();
        var createRequest = new CreateVehicleRequest("GHI3456", "Nissan Altima", 2020, 15000);
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", createRequest);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(createdVehicle);

        var updateRequest = new UpdateVehicleRequest("Nissan Altima Updated", 2020, 20000);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/vehicles/{createdVehicle.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedVehicle = await response.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(updatedVehicle);
        Assert.Equal("Nissan Altima Updated", updatedVehicle.Model);
        Assert.Equal(20000, updatedVehicle.Mileage);
    }

    [Fact]
    public async Task Delete_ExistingVehicle_ReturnsNoContent()
    {
        // Arrange
        await ClearDatabase();
        var createRequest = new CreateVehicleRequest("JKL7890", "Mazda 3", 2019, 25000);
        var createResponse = await _client.PostAsJsonAsync("/api/vehicles", createRequest);
        var createdVehicle = await createResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(createdVehicle);

        // Act
        var response = await _client.DeleteAsync($"/api/vehicles/{createdVehicle.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify vehicle is deleted
        var getResponse = await _client.GetAsync($"/api/vehicles/{createdVehicle.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
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
