using System.Net;
using System.Net.Http.Json;
using FleetManager.Application.DTOs;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetManager.Tests.Integration;

public class TripsControllerTests : IClassFixture<FleetManagerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly FleetManagerWebApplicationFactory _factory;

    public TripsControllerTests(FleetManagerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StartTrip_ValidRequest_CreatesTripAndUpdatesVehicleStatus()
    {
        // Arrange
        await ClearDatabase();
        
        // Create a vehicle
        var vehicleRequest = new CreateVehicleRequest("TRP1234", "Toyota Camry", 2023, 1000);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle);

        // Create a driver
        var driverRequest = new CreateDriverRequest("John Doe", "DL123456", "555-1234");
        var driverResponse = await _client.PostAsJsonAsync("/api/drivers", driverRequest);
        var driver = await driverResponse.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(driver);

        var tripRequest = new StartTripRequest(vehicle.Id, driver.Id, "City A to City B");

        // Act
        var response = await _client.PostAsJsonAsync("/api/trips/start", tripRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var trip = await response.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(trip);
        Assert.Equal(vehicle.Id, trip.VehicleId);
        Assert.Equal(driver.Id, trip.DriverId);
        Assert.Equal("City A to City B", trip.Route);
        Assert.Null(trip.EndDate);

        // Verify vehicle status is updated to InUse
        var vehicleCheckResponse = await _client.GetAsync($"/api/vehicles/{vehicle.Id}");
        var updatedVehicle = await vehicleCheckResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(updatedVehicle);
        Assert.Equal("InUse", updatedVehicle.Status);
    }

    [Fact]
    public async Task EndTrip_ValidRequest_FinalizesTrip()
    {
        // Arrange
        await ClearDatabase();
        
        // Create a vehicle
        var vehicleRequest = new CreateVehicleRequest("TRP5678", "Honda Accord", 2022, 5000);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle);

        // Create a driver
        var driverRequest = new CreateDriverRequest("Jane Smith", "DL789012", "555-5678");
        var driverResponse = await _client.PostAsJsonAsync("/api/drivers", driverRequest);
        var driver = await driverResponse.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(driver);

        // Start a trip
        var startTripRequest = new StartTripRequest(vehicle.Id, driver.Id, "City C to City D");
        var startTripResponse = await _client.PostAsJsonAsync("/api/trips/start", startTripRequest);
        var startedTrip = await startTripResponse.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(startedTrip);

        var endTripRequest = new EndTripRequest(150);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/trips/end/{startedTrip.Id}", endTripRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var endedTrip = await response.Content.ReadFromJsonAsync<TripResponse>();
        Assert.NotNull(endedTrip);
        Assert.NotNull(endedTrip.EndDate);
        Assert.Equal(150, endedTrip.Distance);

        // Verify vehicle status is updated to Available and mileage is updated
        var vehicleCheckResponse = await _client.GetAsync($"/api/vehicles/{vehicle.Id}");
        var updatedVehicle = await vehicleCheckResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(updatedVehicle);
        Assert.Equal("Available", updatedVehicle.Status);
        Assert.Equal(5150, updatedVehicle.Mileage);
    }

    [Fact]
    public async Task StartTrip_InvalidVehicleId_ReturnsBadRequest()
    {
        // Arrange
        await ClearDatabase();
        
        // Create a driver
        var driverRequest = new CreateDriverRequest("Bob Johnson", "DL345678", "555-9012");
        var driverResponse = await _client.PostAsJsonAsync("/api/drivers", driverRequest);
        var driver = await driverResponse.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(driver);

        var tripRequest = new StartTripRequest(Guid.NewGuid(), driver.Id, "Invalid Route");

        // Act
        var response = await _client.PostAsJsonAsync("/api/trips/start", tripRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StartTrip_VehicleAlreadyInUse_ReturnsBadRequest()
    {
        // Arrange
        await ClearDatabase();
        
        // Create a vehicle
        var vehicleRequest = new CreateVehicleRequest("TRP9012", "Ford Fusion", 2021, 8000);
        var vehicleResponse = await _client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleResponse>();
        Assert.NotNull(vehicle);

        // Create two drivers
        var driver1Request = new CreateDriverRequest("Driver One", "DL111111", "555-1111");
        var driver1Response = await _client.PostAsJsonAsync("/api/drivers", driver1Request);
        var driver1 = await driver1Response.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(driver1);

        var driver2Request = new CreateDriverRequest("Driver Two", "DL222222", "555-2222");
        var driver2Response = await _client.PostAsJsonAsync("/api/drivers", driver2Request);
        var driver2 = await driver2Response.Content.ReadFromJsonAsync<DriverResponse>();
        Assert.NotNull(driver2);

        // Start first trip
        var trip1Request = new StartTripRequest(vehicle.Id, driver1.Id, "First Trip");
        await _client.PostAsJsonAsync("/api/trips/start", trip1Request);

        // Try to start second trip with same vehicle
        var trip2Request = new StartTripRequest(vehicle.Id, driver2.Id, "Second Trip");

        // Act
        var response = await _client.PostAsJsonAsync("/api/trips/start", trip2Request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
