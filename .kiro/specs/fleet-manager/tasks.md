# Implementation Plan

- [x] 1. Set up project structure and solution









  - Create solution file and project structure following Clean Architecture
  - Add projects: FleetManager.Domain, FleetManager.Application, FleetManager.Infrastructure, FleetManager.Api, FleetManager.Tests
  - Configure project references between layers
  - Add required NuGet packages to each project
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [ ] 2. Implement domain layer entities and enums
  - [ ] 2.1 Create VehicleStatus enum
    - Define Available, InUse, and InMaintenance status values
    - _Requirements: 1.3, 8.1, 8.2, 8.3_
  
  - [ ] 2.2 Implement Vehicle entity with business logic
    - Create Vehicle entity with all properties (Id, Plate, Model, Year, Mileage, LastMaintenanceDate, NextMaintenanceDate, Status)
    - Implement UpdateMileage, StartTrip, EndTrip, StartMaintenance, CompleteMaintenance methods
    - Implement CalculateNextMaintenanceDate and IsMaintenanceDue methods
    - Add navigation properties for MaintenanceRecords and Trips
    - _Requirements: 1.3, 4.1, 4.2, 4.3, 8.1, 8.2, 8.3, 8.4_
  
  - [ ] 2.3 Implement Driver entity with business logic
    - Create Driver entity with all properties (Id, Name, LicenseNumber, Phone, Active)
    - Implement Activate, Deactivate, and IsAvailable methods
    - Add navigation property for Trips
    - _Requirements: 2.3, 2.5_
  
  - [ ] 2.4 Implement MaintenanceRecord entity
    - Create MaintenanceRecord entity with all properties (Id, VehicleId, Date, Description, Cost)
    - Add navigation property to Vehicle
    - _Requirements: 3.3_
  
  - [ ] 2.5 Implement Trip entity with business logic
    - Create Trip entity with all properties (Id, VehicleId, DriverId, Route, StartDate, EndDate, Distance)
    - Implement End method to finalize trip
    - Implement IsActive method to check if trip is ongoing
    - Add navigation properties to Vehicle and Driver
    - _Requirements: 6.3, 6.4, 6.5_

- [ ] 3. Define repository interfaces in domain layer
  - Create IVehicleRepository interface with methods: GetByIdAsync, GetAllAsync, GetAvailableAsync, GetVehiclesWithUpcomingMaintenanceAsync, GetByPlateAsync, AddAsync, UpdateAsync, DeleteAsync
  - Create IDriverRepository interface with methods: GetByIdAsync, GetAllAsync, GetAvailableAsync, GetByLicenseNumberAsync, AddAsync, UpdateAsync, DeleteAsync
  - Create IMaintenanceRecordRepository interface with methods: GetByIdAsync, GetByVehicleIdAsync, AddAsync
  - Create ITripRepository interface with methods: GetByIdAsync, GetAllAsync, GetActiveTripsAsync, GetByVehicleIdAsync, GetByDriverIdAsync, AddAsync, UpdateAsync
  - _Requirements: 9.2_

- [ ] 4. Implement infrastructure layer - database setup
  - [ ] 4.1 Create FleetManagerDbContext
    - Define DbContext with DbSet properties for all entities
    - Configure OnModelCreating to apply entity configurations
    - _Requirements: 1.5, 2.5, 3.5_
  
  - [ ] 4.2 Create entity configurations
    - Implement VehicleConfiguration with property constraints, indexes, and relationships
    - Implement DriverConfiguration with property constraints and indexes
    - Implement MaintenanceRecordConfiguration with relationships
    - Implement TripConfiguration with relationships
    - _Requirements: 1.5, 2.5, 3.5_
  
  - [ ] 4.3 Create initial database migration
    - Generate EF Core migration for initial schema
    - Verify migration creates all tables, indexes, and foreign keys correctly
    - _Requirements: 1.5, 2.5, 3.5_

- [ ] 5. Implement repository pattern in infrastructure layer
  - [ ] 5.1 Implement VehicleRepository
    - Implement all IVehicleRepository methods using EF Core
    - Implement GetVehiclesWithUpcomingMaintenanceAsync with date filtering logic
    - Include proper error handling and null checks
    - _Requirements: 1.1, 1.4, 4.4, 7.1, 7.3_
  
  - [ ] 5.2 Implement DriverRepository
    - Implement all IDriverRepository methods using EF Core
    - Implement GetAvailableAsync filtering by Active status
    - Include proper error handling and null checks
    - _Requirements: 2.1, 2.4, 7.2_
  
  - [ ] 5.3 Implement MaintenanceRecordRepository
    - Implement all IMaintenanceRecordRepository methods using EF Core
    - Include proper error handling and null checks
    - _Requirements: 3.1, 3.5_
  
  - [ ] 5.4 Implement TripRepository
    - Implement all ITripRepository methods using EF Core
    - Implement GetActiveTripsAsync filtering by null EndDate
    - Include proper error handling and null checks
    - _Requirements: 6.1_

- [ ] 6. Implement application layer - DTOs and mappings
  - [ ] 6.1 Create request DTOs
    - Create CreateVehicleRequest, UpdateVehicleRequest records
    - Create CreateDriverRequest record
    - Create CreateMaintenanceRecordRequest record
    - Create StartTripRequest, EndTripRequest records
    - _Requirements: 1.1, 2.1, 3.1, 6.1_
  
  - [ ] 6.2 Create response DTOs
    - Create VehicleResponse, DriverResponse, MaintenanceRecordResponse, TripResponse records
    - _Requirements: 1.4, 2.4, 3.5, 6.4_
  
  - [ ] 6.3 Create AutoMapper profiles
    - Create mapping profiles for all entity-to-DTO conversions
    - Configure custom mappings for enum-to-string conversions
    - _Requirements: 1.4, 2.4, 3.5, 6.4_

- [ ] 7. Implement application layer - services
  - [ ] 7.1 Implement VehicleService
    - Implement GetByIdAsync, GetAllAsync, GetAvailableAsync, GetUpcomingMaintenanceAsync methods
    - Implement CreateAsync with plate uniqueness validation
    - Implement UpdateAsync and DeleteAsync methods
    - Use AutoMapper for entity-DTO conversions
    - _Requirements: 1.1, 1.2, 1.4, 4.4, 7.1, 7.3_
  
  - [ ] 7.2 Implement DriverService
    - Implement GetByIdAsync, GetAllAsync, GetAvailableAsync methods
    - Implement CreateAsync with license number uniqueness validation
    - Implement UpdateAsync, DeleteAsync, ActivateAsync, DeactivateAsync methods
    - Use AutoMapper for entity-DTO conversions
    - _Requirements: 2.1, 2.2, 2.4, 2.5, 7.2_
  
  - [ ] 7.3 Implement MaintenanceService
    - Implement CreateAsync method that creates maintenance record and updates vehicle
    - Update vehicle's LastMaintenanceDate and calculate NextMaintenanceDate
    - Update vehicle status to InMaintenance
    - Implement GetByVehicleIdAsync method
    - Use AutoMapper for entity-DTO conversions
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 4.1, 4.2, 8.3_
  
  - [ ] 7.4 Implement TripService
    - Implement StartTripAsync with vehicle and driver availability validation
    - Update vehicle status to InUse when trip starts
    - Implement EndTripAsync to finalize trip, calculate distance, update mileage
    - Update vehicle status to Available when trip ends
    - Implement GetByIdAsync, GetAllAsync, GetActiveTripsAsync methods
    - Use AutoMapper for entity-DTO conversions
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 8.1, 8.2_

- [ ] 8. Implement exception handling
  - Create custom exception classes: FleetManagerException, EntityNotFoundException, DuplicateEntityException, InvalidOperationException
  - Implement GlobalExceptionHandler to map exceptions to HTTP status codes
  - Add structured error responses with appropriate status codes
  - _Requirements: 1.2, 2.2, 6.2_

- [ ] 9. Implement API layer - controllers
  - [ ] 9.1 Implement VehiclesController
    - Create GET /api/vehicles endpoint to list all vehicles
    - Create GET /api/vehicles/{id} endpoint to get vehicle by ID
    - Create POST /api/vehicles endpoint to create new vehicle
    - Create PUT /api/vehicles/{id} endpoint to update vehicle
    - Create DELETE /api/vehicles/{id} endpoint to delete vehicle
    - Add proper HTTP status codes and error handling
    - _Requirements: 1.1, 1.4_
  
  - [ ] 9.2 Implement DriversController
    - Create GET /api/drivers endpoint to list all drivers
    - Create GET /api/drivers/{id} endpoint to get driver by ID
    - Create GET /api/drivers/available endpoint to list available drivers
    - Create POST /api/drivers endpoint to create new driver
    - Create PUT /api/drivers/{id} endpoint to update driver
    - Create DELETE /api/drivers/{id} endpoint to delete driver
    - Create POST /api/drivers/{id}/activate and /deactivate endpoints
    - Add proper HTTP status codes and error handling
    - _Requirements: 2.1, 2.4, 7.2_
  
  - [ ] 9.3 Implement MaintenanceController
    - Create GET /api/maintenance/upcoming endpoint to list vehicles with upcoming maintenance
    - Create POST /api/maintenance endpoint to create maintenance record
    - Add proper HTTP status codes and error handling
    - _Requirements: 3.1, 4.4_
  
  - [ ] 9.4 Implement TripsController
    - Create POST /api/trips/start endpoint to start a trip
    - Create POST /api/trips/end/{id} endpoint to end a trip
    - Create GET /api/trips endpoint to list all trips
    - Create GET /api/trips/{id} endpoint to get trip by ID
    - Add proper HTTP status codes and error handling
    - _Requirements: 6.1, 6.4_

- [ ] 10. Implement Quartz.NET background job
  - [ ] 10.1 Create MaintenanceCheckJob
    - Implement IJob interface with Execute method
    - Inject IVehicleRepository and ILogger dependencies
    - Query vehicles with upcoming maintenance within 3 days threshold
    - Log alert messages for each vehicle with upcoming maintenance
    - _Requirements: 5.2, 5.3, 5.5_
  
  - [ ] 10.2 Configure Quartz.NET in Program.cs
    - Register Quartz services with dependency injection
    - Configure MaintenanceCheckJob with daily cron schedule (midnight)
    - Add Quartz hosted service to run jobs
    - _Requirements: 5.1, 5.4_

- [ ] 11. Configure dependency injection and middleware
  - Configure DbContext with SQL Server connection string in Program.cs
  - Register all repositories with scoped lifetime
  - Register all application services with scoped lifetime
  - Register AutoMapper
  - Add GlobalExceptionHandler to exception handling pipeline
  - Configure Swagger/OpenAPI for API documentation
  - Add CORS policy if needed
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [ ] 12. Implement Redis caching (optional enhancement)
  - Create ICacheService interface
  - Implement RedisCacheService using StackExchange.Redis
  - Add caching to VehicleService.GetAvailableAsync method
  - Add caching to DriverService.GetAvailableAsync method
  - Implement cache invalidation on entity updates
  - Configure Redis connection in Program.cs
  - _Requirements: 7.1, 7.2_

- [ ] 13. Configure logging with Serilog
  - Install Serilog packages (Serilog.AspNetCore, Serilog.Sinks.Console, Serilog.Sinks.Seq)
  - Configure Serilog in Program.cs with Console and Seq sinks
  - Add structured logging to all services and jobs
  - Configure log levels in appsettings.json
  - _Requirements: 5.5_

- [ ] 14. Create Docker configuration
  - Create Dockerfile for FleetManager.Api project
  - Create docker-compose.yml with services: api, sqlserver, redis, seq
  - Configure environment variables for connection strings
  - Add volume mappings for data persistence
  - Test Docker Compose build and run
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [ ] 15. Write unit tests for domain entities
  - [ ] 15.1 Write Vehicle entity tests
    - Test UpdateMileage method
    - Test status transition methods (StartTrip, EndTrip, StartMaintenance, CompleteMaintenance)
    - Test CalculateNextMaintenanceDate method
    - Test IsMaintenanceDue method with various date scenarios
    - _Requirements: 4.1, 4.2, 8.1, 8.2, 8.3, 8.4, 10.4_
  
  - [ ] 15.2 Write Driver entity tests
    - Test Activate and Deactivate methods
    - Test IsAvailable method
    - _Requirements: 2.5, 10.4_
  
  - [ ] 15.3 Write Trip entity tests
    - Test End method
    - Test IsActive method
    - _Requirements: 6.4, 10.4_

- [ ] 16. Write unit tests for application services
  - [ ] 16.1 Write VehicleService tests
    - Test CreateAsync with valid and duplicate plate scenarios
    - Test GetAvailableAsync filtering logic
    - Test GetUpcomingMaintenanceAsync date filtering
    - Mock repository dependencies using Moq
    - _Requirements: 1.2, 4.4, 7.1, 10.1, 10.4_
  
  - [ ] 16.2 Write MaintenanceService tests
    - Test CreateAsync updates vehicle maintenance dates correctly
    - Test CreateAsync updates vehicle status to InMaintenance
    - Mock repository dependencies using Moq
    - _Requirements: 3.2, 3.4, 4.1, 4.2, 8.3, 10.1, 10.4_
  
  - [ ] 16.3 Write TripService tests
    - Test StartTripAsync validates vehicle and driver availability
    - Test StartTripAsync updates vehicle status to InUse
    - Test EndTripAsync updates vehicle mileage and status
    - Mock repository dependencies using Moq
    - _Requirements: 6.2, 6.3, 6.5, 8.1, 8.2, 10.1, 10.4_

- [ ] 17. Write integration tests for API endpoints
  - [ ] 17.1 Write VehiclesController integration tests
    - Test POST /api/vehicles creates vehicle and returns 201
    - Test GET /api/vehicles returns all vehicles
    - Test GET /api/vehicles/{id} returns 404 for non-existent vehicle
    - Test PUT /api/vehicles/{id} updates vehicle
    - Test DELETE /api/vehicles/{id} deletes vehicle
    - Use WebApplicationFactory for in-memory testing
    - _Requirements: 1.1, 1.4, 10.2_
  
  - [ ] 17.2 Write TripsController integration tests
    - Test POST /api/trips/start creates trip and updates vehicle status
    - Test POST /api/trips/end/{id} finalizes trip and updates mileage
    - Test validation errors return 400 Bad Request
    - Use WebApplicationFactory for in-memory testing
    - _Requirements: 6.1, 6.4, 6.5, 10.2_
  
  - [ ] 17.3 Write MaintenanceController integration tests
    - Test POST /api/maintenance creates record and updates vehicle
    - Test GET /api/maintenance/upcoming returns filtered vehicles
    - Use WebApplicationFactory for in-memory testing
    - _Requirements: 3.1, 4.4, 10.2_

- [ ] 18. Write tests for Quartz.NET job
  - Test MaintenanceCheckJob.Execute method
  - Mock IVehicleRepository to return vehicles with upcoming maintenance
  - Verify log messages are generated for each vehicle
  - Verify job executes without errors
  - _Requirements: 5.2, 5.3, 5.5, 10.3, 10.5_

- [ ] 19. Create configuration files
  - Create appsettings.json with connection strings, Redis config, Quartz config, Serilog config
  - Create appsettings.Development.json with development-specific settings
  - Create appsettings.Production.json with production-specific settings
  - Add maintenance interval configuration
  - _Requirements: 4.1, 4.2, 5.1_

- [ ] 20. Create README documentation
  - Document project overview and architecture
  - Document prerequisites and setup instructions
  - Document how to run with Docker Compose
  - Document API endpoints with example requests/responses
  - Document database migration commands
  - Document testing instructions
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_
