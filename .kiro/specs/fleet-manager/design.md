# FleetManager - Design Document

## Overview

O FleetManager é um sistema de gestão de frota construído com ASP.NET Core 8 seguindo os princípios de Clean Architecture. O sistema gerencia veículos, condutores, manutenções e viagens, com automação de alertas de manutenção preventiva através de jobs agendados.

### Key Design Principles

- **Clean Architecture**: Separação clara entre camadas de domínio, aplicação, infraestrutura e API
- **Domain-Driven Design**: Entidades ricas com lógica de negócio encapsulada
- **Repository Pattern**: Abstração da camada de acesso a dados
- **CQRS Light**: Separação entre comandos e queries na camada de aplicação
- **Dependency Injection**: Inversão de controle para todas as dependências

## Architecture

### Layer Structure

```
/src
├── FleetManager.Api              # Presentation Layer
│   ├── Controllers/              # REST API Controllers
│   ├── Middleware/               # Custom middleware
│   └── Program.cs                # Application entry point
│
├── FleetManager.Application      # Application Layer
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Application service interfaces
│   ├── Services/                 # Application services (use cases)
│   └── Mappings/                 # AutoMapper profiles
│
├── FleetManager.Domain           # Domain Layer
│   ├── Entities/                 # Domain entities
│   ├── Enums/                    # Domain enumerations
│   ├── Interfaces/               # Repository interfaces
│   └── ValueObjects/             # Value objects
│
├── FleetManager.Infrastructure   # Infrastructure Layer
│   ├── Data/                     # EF Core DbContext
│   ├── Repositories/             # Repository implementations
│   ├── Jobs/                     # Quartz.NET jobs
│   ├── Cache/                    # Redis cache implementation
│   └── Logging/                  # Serilog configuration
│
└── FleetManager.Tests            # Test Layer
    ├── Unit/                     # Unit tests
    └── Integration/              # Integration tests
```

### Technology Stack

- **Backend**: ASP.NET Core 8 Web API
- **Frontend**: Blazor Server (future phase)
- **Database**: SQL Server with Entity Framework Core 8
- **Background Jobs**: Quartz.NET
- **Caching**: Redis
- **Logging**: Serilog + Seq
- **Testing**: xUnit + Moq
- **CI/CD**: GitHub Actions
- **Containerization**: Docker + Docker Compose

## Components and Interfaces

### Domain Layer

#### Entities

**Vehicle Entity**
```csharp
public class Vehicle
{
    public Guid Id { get; private set; }
    public string Plate { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public int Mileage { get; private set; }
    public DateTime LastMaintenanceDate { get; private set; }
    public DateTime NextMaintenanceDate { get; private set; }
    public VehicleStatus Status { get; private set; }
    
    // Navigation properties
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; private set; }
    public ICollection<Trip> Trips { get; private set; }
    
    // Business methods
    public void UpdateMileage(int newMileage);
    public void StartTrip();
    public void EndTrip(int distanceTraveled);
    public void StartMaintenance();
    public void CompleteMaintenance(DateTime maintenanceDate);
    public void CalculateNextMaintenanceDate(int daysInterval);
    public bool IsMaintenanceDue(int daysThreshold);
}
```

**Driver Entity**
```csharp
public class Driver
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public string Phone { get; private set; }
    public bool Active { get; private set; }
    
    // Navigation properties
    public ICollection<Trip> Trips { get; private set; }
    
    // Business methods
    public void Activate();
    public void Deactivate();
    public bool IsAvailable();
}
```

**MaintenanceRecord Entity**
```csharp
public class MaintenanceRecord
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    public decimal Cost { get; private set; }
    
    // Navigation properties
    public Vehicle Vehicle { get; private set; }
}
```

**Trip Entity**
```csharp
public class Trip
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid DriverId { get; private set; }
    public string Route { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public int Distance { get; private set; }
    
    // Navigation properties
    public Vehicle Vehicle { get; private set; }
    public Driver Driver { get; private set; }
    
    // Business methods
    public void End(int distance);
    public bool IsActive();
}
```

#### Enums

```csharp
public enum VehicleStatus
{
    Available = 0,
    InUse = 1,
    InMaintenance = 2
}
```

#### Repository Interfaces

```csharp
public interface IVehicleRepository
{
    Task<Vehicle> GetByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<IEnumerable<Vehicle>> GetAvailableAsync();
    Task<IEnumerable<Vehicle>> GetVehiclesWithUpcomingMaintenanceAsync(int daysThreshold);
    Task<Vehicle> GetByPlateAsync(string plate);
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Guid id);
}

public interface IDriverRepository
{
    Task<Driver> GetByIdAsync(Guid id);
    Task<IEnumerable<Driver>> GetAllAsync();
    Task<IEnumerable<Driver>> GetAvailableAsync();
    Task<Driver> GetByLicenseNumberAsync(string licenseNumber);
    Task AddAsync(Driver driver);
    Task UpdateAsync(Driver driver);
    Task DeleteAsync(Guid id);
}

public interface IMaintenanceRecordRepository
{
    Task<MaintenanceRecord> GetByIdAsync(Guid id);
    Task<IEnumerable<MaintenanceRecord>> GetByVehicleIdAsync(Guid vehicleId);
    Task AddAsync(MaintenanceRecord record);
}

public interface ITripRepository
{
    Task<Trip> GetByIdAsync(Guid id);
    Task<IEnumerable<Trip>> GetAllAsync();
    Task<IEnumerable<Trip>> GetActiveTripsAsync();
    Task<IEnumerable<Trip>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Trip>> GetByDriverIdAsync(Guid driverId);
    Task AddAsync(Trip trip);
    Task UpdateAsync(Trip trip);
}
```

### Application Layer

#### DTOs

```csharp
// Request DTOs
public record CreateVehicleRequest(string Plate, string Model, int Year, int Mileage);
public record UpdateVehicleRequest(string Model, int Year, int Mileage);
public record CreateDriverRequest(string Name, string LicenseNumber, string Phone);
public record CreateMaintenanceRecordRequest(Guid VehicleId, DateTime Date, string Description, decimal Cost);
public record StartTripRequest(Guid VehicleId, Guid DriverId, string Route);
public record EndTripRequest(int Distance);

// Response DTOs
public record VehicleResponse(Guid Id, string Plate, string Model, int Year, int Mileage, 
    DateTime LastMaintenanceDate, DateTime NextMaintenanceDate, string Status);
public record DriverResponse(Guid Id, string Name, string LicenseNumber, string Phone, bool Active);
public record MaintenanceRecordResponse(Guid Id, Guid VehicleId, DateTime Date, string Description, decimal Cost);
public record TripResponse(Guid Id, Guid VehicleId, Guid DriverId, string Route, 
    DateTime StartDate, DateTime? EndDate, int Distance);
```

#### Application Services

```csharp
public interface IVehicleService
{
    Task<VehicleResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<VehicleResponse>> GetAllAsync();
    Task<IEnumerable<VehicleResponse>> GetAvailableAsync();
    Task<IEnumerable<VehicleResponse>> GetUpcomingMaintenanceAsync(int daysThreshold);
    Task<VehicleResponse> CreateAsync(CreateVehicleRequest request);
    Task<VehicleResponse> UpdateAsync(Guid id, UpdateVehicleRequest request);
    Task DeleteAsync(Guid id);
}

public interface IDriverService
{
    Task<DriverResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<DriverResponse>> GetAllAsync();
    Task<IEnumerable<DriverResponse>> GetAvailableAsync();
    Task<DriverResponse> CreateAsync(CreateDriverRequest request);
    Task<DriverResponse> UpdateAsync(Guid id, CreateDriverRequest request);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}

public interface IMaintenanceService
{
    Task<MaintenanceRecordResponse> CreateAsync(CreateMaintenanceRecordRequest request);
    Task<IEnumerable<MaintenanceRecordResponse>> GetByVehicleIdAsync(Guid vehicleId);
}

public interface ITripService
{
    Task<TripResponse> StartTripAsync(StartTripRequest request);
    Task<TripResponse> EndTripAsync(Guid tripId, EndTripRequest request);
    Task<TripResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<TripResponse>> GetAllAsync();
    Task<IEnumerable<TripResponse>> GetActiveTripsAsync();
}
```

### Infrastructure Layer

#### DbContext

```csharp
public class FleetManagerDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<Trip> Trips { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetManagerDbContext).Assembly);
    }
}
```

#### Quartz.NET Job

```csharp
public class MaintenanceCheckJob : IJob
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<MaintenanceCheckJob> _logger;
    private const int DaysThreshold = 3;
    
    public async Task Execute(IJobExecutionContext context)
    {
        var vehicles = await _vehicleRepository.GetVehiclesWithUpcomingMaintenanceAsync(DaysThreshold);
        
        foreach (var vehicle in vehicles)
        {
            _logger.LogInformation(
                "[ALERTA] Veículo {Plate} com manutenção próxima ({NextMaintenanceDate:dd/MM/yyyy})",
                vehicle.Plate,
                vehicle.NextMaintenanceDate);
        }
        
        _logger.LogInformation("MaintenanceCheckJob executado. {Count} veículos com manutenção próxima.", 
            vehicles.Count());
    }
}
```

#### Cache Service

```csharp
public interface ICacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    
    // Implementation using StackExchange.Redis
}
```

### API Layer

#### Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetAll();
    
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleResponse>> GetById(Guid id);
    
    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create(CreateVehicleRequest request);
    
    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleResponse>> Update(Guid id, UpdateVehicleRequest request);
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id);
}

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    // Similar structure to VehiclesController
}

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetUpcoming();
    
    [HttpPost]
    public async Task<ActionResult<MaintenanceRecordResponse>> Create(CreateMaintenanceRecordRequest request);
}

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<TripResponse>> Start(StartTripRequest request);
    
    [HttpPost("end/{id}")]
    public async Task<ActionResult<TripResponse>> End(Guid id, EndTripRequest request);
}
```

## Data Models

### Database Schema

```sql
-- Vehicles Table
CREATE TABLE Vehicles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Plate NVARCHAR(10) NOT NULL UNIQUE,
    Model NVARCHAR(100) NOT NULL,
    Year INT NOT NULL,
    Mileage INT NOT NULL DEFAULT 0,
    LastMaintenanceDate DATETIME NOT NULL,
    NextMaintenanceDate DATETIME NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- Drivers Table
CREATE TABLE Drivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    LicenseNumber NVARCHAR(20) NOT NULL UNIQUE,
    Phone NVARCHAR(15) NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- MaintenanceRecords Table
CREATE TABLE MaintenanceRecords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VehicleId UNIQUEIDENTIFIER NOT NULL,
    Date DATETIME NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Cost DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
);

-- Trips Table
CREATE TABLE Trips (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VehicleId UNIQUEIDENTIFIER NOT NULL,
    DriverId UNIQUEIDENTIFIER NOT NULL,
    Route NVARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    Distance INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id),
    FOREIGN KEY (DriverId) REFERENCES Drivers(Id)
);

-- Indexes
CREATE INDEX IX_Vehicles_Status ON Vehicles(Status);
CREATE INDEX IX_Vehicles_NextMaintenanceDate ON Vehicles(NextMaintenanceDate);
CREATE INDEX IX_Drivers_Active ON Drivers(Active);
CREATE INDEX IX_MaintenanceRecords_VehicleId ON MaintenanceRecords(VehicleId);
CREATE INDEX IX_Trips_VehicleId ON Trips(VehicleId);
CREATE INDEX IX_Trips_DriverId ON Trips(DriverId);
CREATE INDEX IX_Trips_EndDate ON Trips(EndDate);
```

### Entity Framework Configurations

```csharp
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Plate).IsRequired().HasMaxLength(10);
        builder.HasIndex(v => v.Plate).IsUnique();
        builder.Property(v => v.Model).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Status).HasConversion<int>();
        
        builder.HasMany(v => v.MaintenanceRecords)
            .WithOne(m => m.Vehicle)
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(v => v.Trips)
            .WithOne(t => t.Vehicle)
            .HasForeignKey(t => t.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## Error Handling

### Exception Hierarchy

```csharp
public class FleetManagerException : Exception
{
    public FleetManagerException(string message) : base(message) { }
}

public class EntityNotFoundException : FleetManagerException
{
    public EntityNotFoundException(string entityName, Guid id) 
        : base($"{entityName} with ID {id} not found") { }
}

public class DuplicateEntityException : FleetManagerException
{
    public DuplicateEntityException(string entityName, string field, string value)
        : base($"{entityName} with {field} '{value}' already exists") { }
}

public class InvalidOperationException : FleetManagerException
{
    public InvalidOperationException(string message) : base(message) { }
}
```

### Global Exception Handler

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
        
        var (statusCode, message) = exception switch
        {
            EntityNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DuplicateEntityException => (StatusCodes.Status409Conflict, exception.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An internal error occurred")
        };
        
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new { error = message }, cancellationToken);
        
        return true;
    }
}
```

## Testing Strategy

### Unit Tests

**Domain Entity Tests**
- Vehicle status transitions
- Maintenance date calculations
- Business rule validations
- Trip lifecycle management

**Application Service Tests**
- Service orchestration logic
- DTO mapping
- Validation logic
- Mock repository interactions

### Integration Tests

**API Endpoint Tests**
- Full request/response cycle
- Database interactions
- Authentication/Authorization
- Error handling

**Repository Tests**
- Database CRUD operations
- Query filtering
- Entity relationships
- Transaction handling

**Job Tests**
- MaintenanceCheckJob execution
- Scheduled trigger verification
- Logging verification

### Test Data Builders

```csharp
public class VehicleBuilder
{
    private string _plate = "ABC1234";
    private string _model = "Test Model";
    private int _year = 2023;
    
    public VehicleBuilder WithPlate(string plate)
    {
        _plate = plate;
        return this;
    }
    
    public Vehicle Build()
    {
        return new Vehicle(_plate, _model, _year, 0);
    }
}
```

## Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FleetManager;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "FleetManager:"
  },
  "Quartz": {
    "MaintenanceCheckCron": "0 0 0 * * ?",
    "MaintenanceDaysThreshold": 3
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
    ]
  },
  "Maintenance": {
    "DefaultIntervalDays": 90
  }
}
```

### Dependency Injection Setup

```csharp
// Program.cs
builder.Services.AddDbContext<FleetManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IMaintenanceRecordRepository, MaintenanceRecordRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();

// Services
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<ITripService, TripService>();

// Cache
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:Configuration"]));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    var jobKey = new JobKey("MaintenanceCheckJob");
    q.AddJob<MaintenanceCheckJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MaintenanceCheckJob-trigger")
        .WithCronSchedule(builder.Configuration["Quartz:MaintenanceCheckCron"]));
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
```

## Deployment

### Docker Compose Configuration

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: src/FleetManager.Api/Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=FleetManager;User=sa;Password=YourStrong@Passw0rd
      - Redis__Configuration=redis:6379
    depends_on:
      - sqlserver
      - redis
      - seq

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data

  seq:
    image: datalust/seq:latest
    environment:
      - ACCEPT_EULA=Y
    ports:
      - "5341:80"
    volumes:
      - seq-data:/data

volumes:
  sqlserver-data:
  redis-data:
  seq-data:
```

### CI/CD Pipeline (GitHub Actions)

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
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
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal
    
    - name: Build Docker image
      run: docker build -t fleetmanager:${{ github.sha }} .
```

## Security Considerations

### Authentication & Authorization
- JWT Bearer token authentication (future phase)
- Role-based access control (Admin, Manager, Driver)
- API key authentication for background jobs

### Data Protection
- Sensitive data encryption at rest
- HTTPS enforcement
- SQL injection prevention via parameterized queries (EF Core)
- Input validation and sanitization

### Logging & Monitoring
- Structured logging with Serilog
- PII data masking in logs
- Centralized log aggregation with Seq
- Performance monitoring and alerting

## Performance Optimization

### Caching Strategy
- Cache vehicle availability list (5 minutes TTL)
- Cache driver availability list (5 minutes TTL)
- Invalidate cache on entity updates

### Database Optimization
- Proper indexing on frequently queried columns
- Eager loading for related entities when needed
- Pagination for large result sets
- Connection pooling

### API Optimization
- Response compression
- Async/await throughout
- Minimal API endpoints for high-performance scenarios
