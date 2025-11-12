# Testing Documentation

Complete guide for writing and running tests in FleetManager.

## Table of Contents

- [Test Structure](#test-structure)
- [Running Tests](#running-tests)
- [Unit Tests](#unit-tests)
- [Integration Tests](#integration-tests)
- [Test Coverage](#test-coverage)
- [Best Practices](#best-practices)

---

## Test Structure

The test project follows the same layered architecture as the main application:

```
FleetManager.Tests/
├── Unit/
│   ├── Domain/
│   │   ├── VehicleTests.cs
│   │   ├── DriverTests.cs
│   │   └── TripTests.cs
│   ├── Application/
│   │   ├── VehicleServiceTests.cs
│   │   ├── DriverServiceTests.cs
│   │   ├── MaintenanceServiceTests.cs
│   │   └── TripServiceTests.cs
│   └── Infrastructure/
│       └── MaintenanceCheckJobTests.cs
└── Integration/
    ├── VehiclesControllerTests.cs
    ├── DriversControllerTests.cs
    ├── MaintenanceControllerTests.cs
    └── TripsControllerTests.cs
```

---

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run with Detailed Output

```bash
dotnet test --verbosity detailed
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~VehicleServiceTests"
```

### Run Specific Test Method

```bash
dotnet test --filter "FullyQualifiedName~VehicleServiceTests.CreateAsync_WithValidData_ReturnsVehicleResponse"
```

### Run Only Unit Tests

```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

### Run Only Integration Tests

```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

### Run Tests in Parallel

```bash
dotnet test --parallel
```

### Run Tests with Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## Unit Tests

Unit tests focus on testing individual components in isolation using mocks.

### Domain Entity Tests

Test business logic in domain entities without external dependencies.

**Example: VehicleTests.cs**

```csharp
public class VehicleTests
{
    [Fact]
    public void UpdateMileage_WithValidMileage_UpdatesSuccessfully()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Toyota Corolla", 2023, 10000);
        
        // Act
        vehicle.UpdateMileage(15000);
        
        // Assert
        Assert.Equal(15000, vehicle.Mileage);
    }
    
    [Fact]
    public void StartTrip_WhenAvailable_ChangesStatusToInUse()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Toyota Corolla", 2023, 10000);
        
        // Act
        vehicle.StartTrip();
        
        // Assert
        Assert.Equal(VehicleStatus.InUse, vehicle.Status);
    }
    
    [Fact]
    public void StartTrip_WhenNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Toyota Corolla", 2023, 10000);
        vehicle.StartTrip(); // Already in use
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => vehicle.StartTrip());
    }
    
    [Fact]
    public void CalculateNextMaintenanceDate_WithDefaultInterval_AddsNinetyDays()
    {
        // Arrange
        var vehicle = new Vehicle("ABC1234", "Toyota Corolla", 2023, 10000);
        var maintenanceDate = new DateTime(2024, 1, 1);
        
        // Act
        vehicle.CompleteMaintenance(maintenanceDate);
        vehicle.CalculateNextMaintenanceDate(90);
        
        // Assert
        Assert.Equal(new DateTime(2024, 4, 1), vehicle.NextMaintenanceDate);
    }
}
```

### Application Service Tests

Test service orchestration logic using mocked repositories.

**Example: VehicleServiceTests.cs**

```csharp
public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICacheService> _mockCache;
    private readonly VehicleService _service;
    
    public VehicleServiceTests()
    {
        _mockRepository = new Mock<IVehicleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCache = new Mock<ICacheService>();
        _service = new VehicleService(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockCache.Object);
    }
    
    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsVehicleResponse()
    {
        // Arrange
        var request = new CreateVehicleRequest("ABC1234", "Toyota Corolla", 2023, 0);
        var vehicle = new Vehicle("ABC1234", "Toyota Corolla", 2023, 0);
        var response = new VehicleResponse(
            vehicle.Id, "ABC1234", "Toyota Corolla", 2023, 0,
            vehicle.LastMaintenanceDate, vehicle.NextMaintenanceDate, "Available");
        
        _mockRepository.Setup(r => r.GetByPlateAsync("ABC1234"))
            .ReturnsAsync((Vehicle)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>()))
            .Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<VehicleResponse>(It.IsAny<Vehicle>()))
            .Returns(response);
        
        // Act
        var result = await _service.CreateAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC1234", result.Plate);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync("vehicles:available"), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WithDuplicatePlate_ThrowsDuplicateEntityException()
    {
        // Arrange
        var request = new CreateVehicleRequest("ABC1234", "Toyota Corolla", 2023, 0);
        var existingVehicle = new Vehicle("ABC1234", "Honda Civic", 2022, 10000);
        
        _mockRepository.Setup(r => r.GetByPlateAsync("ABC1234"))
            .ReturnsAsync(existingVehicle);
        
        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _service.CreateAsync(request));
    }
}
```

### Background Job Tests

Test Quartz.NET jobs with mocked dependencies.

**Example: MaintenanceCheckJobTests.cs**

```csharp
public class MaintenanceCheckJobTests
{
    private readonly Mock<IVehicleRepository> _mockRepository;
    private readonly Mock<ILogger<MaintenanceCheckJob>> _mockLogger;
    private readonly MaintenanceCheckJob _job;
    
    public MaintenanceCheckJobTests()
    {
        _mockRepository = new Mock<IVehicleRepository>();
        _mockLogger = new Mock<ILogger<MaintenanceCheckJob>>();
        _job = new MaintenanceCheckJob(_mockRepository.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task Execute_WithUpcomingMaintenance_LogsAlerts()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Vehicle("ABC1234", "Toyota Corolla", 2023, 10000),
            new Vehicle("XYZ5678", "Honda Civic", 2022, 25000)
        };
        
        _mockRepository.Setup(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3))
            .ReturnsAsync(vehicles);
        
        var context = Mock.Of<IJobExecutionContext>();
        
        // Act
        await _job.Execute(context);
        
        // Assert
        _mockRepository.Verify(r => r.GetVehiclesWithUpcomingMaintenanceAsync(3), Times.Once);
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ABC1234")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
```

---

## Integration Tests

Integration tests verify the entire request/response pipeline using an in-memory database.

### Setup

Integration tests use `WebApplicationFactory` to create a test server:

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FleetManagerDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);
            
            // Add in-memory database
            services.AddDbContext<FleetManagerDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });
            
            // Build service provider and create database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FleetManagerDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
```

### Controller Tests

**Example: VehiclesControllerTests.cs**

```csharp
public class VehiclesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    
    public VehiclesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateVehicle_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateVehicleRequest("ABC1234", "Toyota Corolla", 2023, 0);
        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/vehicles", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var vehicle = JsonSerializer.Deserialize<VehicleResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(vehicle);
        Assert.Equal("ABC1234", vehicle.Plate);
    }
    
    [Fact]
    public async Task GetVehicleById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/api/vehicles/{nonExistentId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAvailableVehicles_ReturnsOnlyAvailableVehicles()
    {
        // Arrange
        // Create vehicles with different statuses
        await CreateVehicleAsync("AAA1111", "Available Vehicle");
        var inUseVehicle = await CreateVehicleAsync("BBB2222", "In Use Vehicle");
        await StartTripWithVehicle(inUseVehicle.Id);
        
        // Act
        var response = await _client.GetAsync("/api/vehicles/available");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<VehicleResponse>>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(vehicles);
        Assert.All(vehicles, v => Assert.Equal("Available", v.Status));
    }
    
    private async Task<VehicleResponse> CreateVehicleAsync(string plate, string model)
    {
        var request = new CreateVehicleRequest(plate, model, 2023, 0);
        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");
        
        var response = await _client.PostAsync("/api/vehicles", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<VehicleResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
```

---

## Test Coverage

### Generate Coverage Report

```bash
# Install coverage tool
dotnet tool install --global dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate HTML report
reportgenerator \
  -reports:"**/coverage.opencover.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html

# Open report
start coveragereport/index.html  # Windows
open coveragereport/index.html   # macOS
xdg-open coveragereport/index.html  # Linux
```

### Coverage Goals

- **Domain Layer**: 90%+ coverage
- **Application Layer**: 85%+ coverage
- **Infrastructure Layer**: 70%+ coverage
- **API Layer**: 80%+ coverage

---

## Best Practices

### Naming Conventions

Use descriptive test names that follow the pattern:
```
MethodName_Scenario_ExpectedBehavior
```

Examples:
- `CreateAsync_WithValidData_ReturnsVehicleResponse`
- `StartTrip_WhenNotAvailable_ThrowsInvalidOperationException`
- `GetAvailableVehicles_ReturnsOnlyAvailableVehicles`

### Arrange-Act-Assert Pattern

Structure all tests using the AAA pattern:

```csharp
[Fact]
public void TestMethod()
{
    // Arrange - Set up test data and dependencies
    var vehicle = new Vehicle("ABC1234", "Toyota Corolla", 2023, 0);
    
    // Act - Execute the method being tested
    vehicle.StartTrip();
    
    // Assert - Verify the expected outcome
    Assert.Equal(VehicleStatus.InUse, vehicle.Status);
}
```

### Test Independence

- Each test should be independent and not rely on other tests
- Use fresh test data for each test
- Clean up resources after tests (handled by in-memory database)

### Mock Only External Dependencies

- Mock repositories, external services, and infrastructure
- Don't mock domain entities or value objects
- Don't mock the class under test

### Test Edge Cases

Always test:
- Happy path (valid input)
- Invalid input
- Boundary conditions
- Null values
- Empty collections
- Concurrent operations

### Use Test Data Builders

Create builder classes for complex test data:

```csharp
public class VehicleBuilder
{
    private string _plate = "ABC1234";
    private string _model = "Test Model";
    private int _year = 2023;
    private int _mileage = 0;
    
    public VehicleBuilder WithPlate(string plate)
    {
        _plate = plate;
        return this;
    }
    
    public VehicleBuilder WithMileage(int mileage)
    {
        _mileage = mileage;
        return this;
    }
    
    public Vehicle Build()
    {
        return new Vehicle(_plate, _model, _year, _mileage);
    }
}

// Usage
var vehicle = new VehicleBuilder()
    .WithPlate("XYZ9999")
    .WithMileage(50000)
    .Build();
```

### Verify Mock Interactions

Always verify that mocked methods were called as expected:

```csharp
_mockRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
_mockCache.Verify(c => c.RemoveAsync("vehicles:available"), Times.Once);
```

---

## Continuous Integration

### GitHub Actions Example

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
    
    - name: Upload coverage
      uses: codecov/codecov-action@v3
      with:
        files: '**/coverage.opencover.xml'
```

---

## Troubleshooting

### Tests Failing Intermittently

- Check for race conditions in async code
- Ensure tests are independent
- Use `Task.Delay` sparingly and only when necessary

### In-Memory Database Issues

- Ensure database is created: `db.Database.EnsureCreated()`
- Clear data between tests if needed
- Use unique database names for parallel test execution

### Mock Setup Not Working

- Verify method signatures match exactly
- Check that mock is configured before method is called
- Use `It.IsAny<T>()` for flexible parameter matching

### Slow Tests

- Reduce test data size
- Use in-memory database instead of real database
- Run tests in parallel: `dotnet test --parallel`
