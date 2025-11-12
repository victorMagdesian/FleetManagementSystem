# Database Documentation

Complete database schema and migration guide for FleetManager.

## Table of Contents

- [Database Schema](#database-schema)
- [Entity Relationships](#entity-relationships)
- [Migrations](#migrations)
- [Seeding Data](#seeding-data)
- [Indexes](#indexes)
- [Useful Queries](#useful-queries)

---

## Database Schema

### Vehicles Table

Stores information about fleet vehicles.

```sql
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
```

**Columns:**
- `Id`: Unique identifier (GUID)
- `Plate`: Vehicle license plate (unique, max 10 characters)
- `Model`: Vehicle model name (max 100 characters)
- `Year`: Manufacturing year
- `Mileage`: Current odometer reading in kilometers
- `LastMaintenanceDate`: Date of last maintenance
- `NextMaintenanceDate`: Calculated date for next maintenance
- `Status`: Vehicle status (0=Available, 1=InUse, 2=InMaintenance)
- `CreatedAt`: Record creation timestamp
- `UpdatedAt`: Record last update timestamp

### Drivers Table

Stores information about authorized drivers.

```sql
CREATE TABLE Drivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    LicenseNumber NVARCHAR(20) NOT NULL UNIQUE,
    Phone NVARCHAR(15) NOT NULL,
    Active BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
```

**Columns:**
- `Id`: Unique identifier (GUID)
- `Name`: Driver's full name (max 100 characters)
- `LicenseNumber`: Driver's license number (unique, max 20 characters)
- `Phone`: Contact phone number (max 15 characters)
- `Active`: Whether driver is active (1=Active, 0=Inactive)
- `CreatedAt`: Record creation timestamp
- `UpdatedAt`: Record last update timestamp

### MaintenanceRecords Table

Stores historical maintenance records for vehicles.

```sql
CREATE TABLE MaintenanceRecords (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    VehicleId UNIQUEIDENTIFIER NOT NULL,
    Date DATETIME NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Cost DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
);
```

**Columns:**
- `Id`: Unique identifier (GUID)
- `VehicleId`: Reference to vehicle (foreign key)
- `Date`: Date when maintenance was performed
- `Description`: Maintenance description (max 500 characters)
- `Cost`: Maintenance cost in currency
- `CreatedAt`: Record creation timestamp

**Relationships:**
- Cascade delete: When a vehicle is deleted, all its maintenance records are deleted

### Trips Table

Stores trip records with vehicle and driver assignments.

```sql
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
```

**Columns:**
- `Id`: Unique identifier (GUID)
- `VehicleId`: Reference to vehicle (foreign key)
- `DriverId`: Reference to driver (foreign key)
- `Route`: Trip route description (max 255 characters)
- `StartDate`: Trip start date and time
- `EndDate`: Trip end date and time (NULL for active trips)
- `Distance`: Trip distance in kilometers
- `CreatedAt`: Record creation timestamp
- `UpdatedAt`: Record last update timestamp

**Relationships:**
- Restrict delete: Vehicles and drivers with trips cannot be deleted

---

## Entity Relationships

```
┌─────────────┐
│  Vehicles   │
│             │
│ - Id        │◄──────────┐
│ - Plate     │           │
│ - Model     │           │ 1:N
│ - Status    │           │
└─────────────┘           │
       ▲                  │
       │                  │
       │ 1:N              │
       │                  │
┌──────┴──────────┐  ┌────┴────────────────┐
│ MaintenanceRecs │  │      Trips          │
│                 │  │                     │
│ - Id            │  │ - Id                │
│ - VehicleId     │  │ - VehicleId         │
│ - Date          │  │ - DriverId          │
│ - Description   │  │ - Route             │
│ - Cost          │  │ - StartDate         │
└─────────────────┘  │ - EndDate           │
                     │ - Distance          │
                     └──────┬──────────────┘
                            │
                            │ N:1
                            │
                            ▼
                     ┌─────────────┐
                     │   Drivers   │
                     │             │
                     │ - Id        │
                     │ - Name      │
                     │ - License   │
                     │ - Active    │
                     └─────────────┘
```

---

## Migrations

### Creating Migrations

Create a new migration when you modify entity models:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

**Example:**

```bash
dotnet ef migrations add AddVehicleColorColumn \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

### Applying Migrations

Apply pending migrations to the database:

```bash
dotnet ef database update \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

### Reverting Migrations

Remove the last migration (if not applied):

```bash
dotnet ef migrations remove \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

Revert to a specific migration:

```bash
dotnet ef database update <MigrationName> \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

### Generating SQL Scripts

Generate SQL script for all migrations:

```bash
dotnet ef migrations script \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api \
  --output migrations.sql
```

Generate SQL script for specific migration range:

```bash
dotnet ef migrations script <FromMigration> <ToMigration> \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api \
  --output migration-range.sql
```

### Listing Migrations

List all migrations:

```bash
dotnet ef migrations list \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

---

## Seeding Data

### Development Seed Data

For development and testing, you can seed initial data:

```csharp
// In FleetManagerDbContext.cs or a separate seeder class

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Seed Vehicles
    modelBuilder.Entity<Vehicle>().HasData(
        new Vehicle("ABC1234", "Toyota Corolla", 2023, 10000),
        new Vehicle("XYZ5678", "Honda Civic", 2022, 25000)
    );
    
    // Seed Drivers
    modelBuilder.Entity<Driver>().HasData(
        new Driver("João Silva", "12345678900", "+5511999999999"),
        new Driver("Maria Santos", "98765432100", "+5511988888888")
    );
}
```

Then create and apply a migration:

```bash
dotnet ef migrations add SeedInitialData \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api

dotnet ef database update \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

---

## Indexes

The following indexes are created for query optimization:

### Vehicles Table

```sql
-- Status index for filtering available vehicles
CREATE INDEX IX_Vehicles_Status ON Vehicles(Status);

-- NextMaintenanceDate index for maintenance alerts
CREATE INDEX IX_Vehicles_NextMaintenanceDate ON Vehicles(NextMaintenanceDate);

-- Unique index on Plate (created automatically)
CREATE UNIQUE INDEX IX_Vehicles_Plate ON Vehicles(Plate);
```

### Drivers Table

```sql
-- Active status index for filtering available drivers
CREATE INDEX IX_Drivers_Active ON Drivers(Active);

-- Unique index on LicenseNumber (created automatically)
CREATE UNIQUE INDEX IX_Drivers_LicenseNumber ON Drivers(LicenseNumber);
```

### MaintenanceRecords Table

```sql
-- VehicleId index for querying maintenance history
CREATE INDEX IX_MaintenanceRecords_VehicleId ON MaintenanceRecords(VehicleId);
```

### Trips Table

```sql
-- VehicleId index for querying vehicle trips
CREATE INDEX IX_Trips_VehicleId ON Trips(VehicleId);

-- DriverId index for querying driver trips
CREATE INDEX IX_Trips_DriverId ON Trips(DriverId);

-- EndDate index for finding active trips
CREATE INDEX IX_Trips_EndDate ON Trips(EndDate);
```

---

## Useful Queries

### Find Available Vehicles

```sql
SELECT * FROM Vehicles
WHERE Status = 0  -- Available
ORDER BY Plate;
```

### Find Vehicles with Upcoming Maintenance

```sql
SELECT * FROM Vehicles
WHERE NextMaintenanceDate <= DATEADD(day, 3, GETDATE())
  AND Status != 2  -- Not in maintenance
ORDER BY NextMaintenanceDate;
```

### Find Active Trips

```sql
SELECT 
    t.Id,
    t.Route,
    t.StartDate,
    v.Plate AS VehiclePlate,
    d.Name AS DriverName
FROM Trips t
INNER JOIN Vehicles v ON t.VehicleId = v.Id
INNER JOIN Drivers d ON t.DriverId = d.Id
WHERE t.EndDate IS NULL
ORDER BY t.StartDate DESC;
```

### Get Maintenance History for a Vehicle

```sql
SELECT 
    m.Date,
    m.Description,
    m.Cost
FROM MaintenanceRecords m
WHERE m.VehicleId = '<vehicle-guid>'
ORDER BY m.Date DESC;
```

### Calculate Total Maintenance Cost per Vehicle

```sql
SELECT 
    v.Plate,
    v.Model,
    COUNT(m.Id) AS MaintenanceCount,
    SUM(m.Cost) AS TotalCost,
    AVG(m.Cost) AS AverageCost
FROM Vehicles v
LEFT JOIN MaintenanceRecords m ON v.Id = m.VehicleId
GROUP BY v.Id, v.Plate, v.Model
ORDER BY TotalCost DESC;
```

### Find Most Active Drivers

```sql
SELECT 
    d.Name,
    d.LicenseNumber,
    COUNT(t.Id) AS TripCount,
    SUM(t.Distance) AS TotalDistance
FROM Drivers d
LEFT JOIN Trips t ON d.Id = t.DriverId
WHERE t.EndDate IS NOT NULL
GROUP BY d.Id, d.Name, d.LicenseNumber
ORDER BY TripCount DESC;
```

### Find Vehicles by Mileage Range

```sql
SELECT 
    Plate,
    Model,
    Year,
    Mileage,
    Status
FROM Vehicles
WHERE Mileage BETWEEN 10000 AND 50000
ORDER BY Mileage DESC;
```

---

## Connection Strings

### Development (Local)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FleetManager;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

### Docker Compose

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver,1433;Database=FleetManager;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

### Production (Environment Variable)

```bash
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=FleetManager;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=True"
```

---

## Database Backup and Restore

### Backup Database

```bash
docker exec fleetmanager-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Passw0rd' \
  -Q "BACKUP DATABASE FleetManager TO DISK='/var/opt/mssql/backup/FleetManager.bak'"
```

### Restore Database

```bash
docker exec fleetmanager-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Passw0rd' \
  -Q "RESTORE DATABASE FleetManager FROM DISK='/var/opt/mssql/backup/FleetManager.bak' WITH REPLACE"
```

---

## Troubleshooting

### Connection Issues

If you can't connect to the database:

1. Check if SQL Server container is running:
```bash
docker ps | grep sqlserver
```

2. Check SQL Server logs:
```bash
docker logs fleetmanager-sqlserver
```

3. Test connection:
```bash
docker exec -it fleetmanager-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Passw0rd' \
  -Q "SELECT @@VERSION"
```

### Migration Errors

If migrations fail:

1. Check current migration status:
```bash
dotnet ef migrations list \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

2. Drop and recreate database (development only):
```bash
dotnet ef database drop --force \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api

dotnet ef database update \
  --project src/FleetManager.Infrastructure \
  --startup-project src/FleetManager.Api
```

### Performance Issues

If queries are slow:

1. Check missing indexes:
```sql
SELECT 
    OBJECT_NAME(s.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE database_id = DB_ID('FleetManager')
ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;
```

2. Update statistics:
```sql
EXEC sp_updatestats;
```

3. Rebuild indexes:
```sql
ALTER INDEX ALL ON Vehicles REBUILD;
ALTER INDEX ALL ON Drivers REBUILD;
ALTER INDEX ALL ON Trips REBUILD;
ALTER INDEX ALL ON MaintenanceRecords REBUILD;
```
