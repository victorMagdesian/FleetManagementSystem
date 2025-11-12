# Configuration Documentation

Complete guide for configuring FleetManager application settings.

## Table of Contents

- [Configuration Files](#configuration-files)
- [Connection Strings](#connection-strings)
- [Cache Settings](#cache-settings)
- [Logging Configuration](#logging-configuration)
- [Quartz.NET Configuration](#quartznet-configuration)
- [Maintenance Settings](#maintenance-settings)
- [Environment Variables](#environment-variables)

---

## Configuration Files

FleetManager uses environment-specific configuration files:

- `appsettings.json` - Base configuration for all environments
- `appsettings.Development.json` - Development-specific overrides
- `appsettings.Production.json` - Production-specific overrides

Configuration files are loaded in order, with later files overriding earlier ones.

---

## Connection Strings

### SQL Server

**Development (appsettings.Development.json):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FleetManager;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

**Production (appsettings.Production.json):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver,1433;Database=FleetManager;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=True"
  }
}
```

**Parameters:**
- `Server`: SQL Server hostname and port
- `Database`: Database name
- `User Id`: SQL Server username
- `Password`: SQL Server password (use environment variable in production)
- `TrustServerCertificate`: Accept self-signed certificates

### Redis

**Development:**

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

**Production:**

```json
{
  "ConnectionStrings": {
    "Redis": "redis:6379"
  }
}
```

**Format:** `hostname:port[,hostname:port]`

For Redis with password:
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379,password=YourRedisPassword"
  }
}
```

---

## Cache Settings

Configure cache expiration times for different data types.

**appsettings.json:**

```json
{
  "CacheSettings": {
    "DefaultExpirationMinutes": 10,
    "AvailableVehiclesCacheMinutes": 5,
    "AvailableDriversCacheMinutes": 5
  }
}
```

**Development (shorter cache for testing):**

```json
{
  "CacheSettings": {
    "DefaultExpirationMinutes": 1,
    "AvailableVehiclesCacheMinutes": 1,
    "AvailableDriversCacheMinutes": 1
  }
}
```

**Settings:**
- `DefaultExpirationMinutes`: Default cache TTL for all cached items
- `AvailableVehiclesCacheMinutes`: Cache TTL for available vehicles list
- `AvailableDriversCacheMinutes`: Cache TTL for available drivers list

---

## Logging Configuration

FleetManager uses Serilog for structured logging with multiple sinks.

### Base Configuration (appsettings.json)

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/fleetmanager-.log",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
          "retainedFileCountLimit": 30
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  }
}
```

### Development Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Information",
        "System": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/fleetmanager-dev-.log",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

### Production Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Error",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "/app/logs/fleetmanager-.log",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
          "retainedFileCountLimit": 90
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:80",
          "apiKey": "${SEQ_API_KEY}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
  }
}
```

### Log Levels

- `Verbose`: Most detailed logs (not used in production)
- `Debug`: Detailed debugging information (development only)
- `Information`: General informational messages
- `Warning`: Warning messages for potentially harmful situations
- `Error`: Error messages for failures
- `Fatal`: Critical errors causing application shutdown

### Sinks

**Console Sink:**
- Outputs logs to console/terminal
- Useful for development and Docker containers

**File Sink:**
- Writes logs to rotating files
- `rollingInterval`: Day, Hour, Month, Year
- `retainedFileCountLimit`: Number of log files to keep

**Seq Sink:**
- Sends structured logs to Seq server
- Provides searchable, queryable log aggregation
- Access at http://localhost:5341

---

## Quartz.NET Configuration

Configure background job scheduling.

**appsettings.json:**

```json
{
  "Quartz": {
    "MaintenanceCheckCron": "0 0 0 * * ?",
    "MaintenanceDaysThreshold": 3
  }
}
```

**Development (more frequent checks):**

```json
{
  "Quartz": {
    "MaintenanceCheckCron": "0 */5 * * * ?",
    "MaintenanceDaysThreshold": 7
  }
}
```

### Cron Expression Format

```
┌───────────── second (0-59)
│ ┌───────────── minute (0-59)
│ │ ┌───────────── hour (0-23)
│ │ │ ┌───────────── day of month (1-31)
│ │ │ │ ┌───────────── month (1-12)
│ │ │ │ │ ┌───────────── day of week (0-6) (Sunday=0)
│ │ │ │ │ │
* * * * * *
```

**Examples:**

| Expression | Description |
|------------|-------------|
| `0 0 0 * * ?` | Daily at midnight |
| `0 0 */6 * * ?` | Every 6 hours |
| `0 */30 * * * ?` | Every 30 minutes |
| `0 0 9 * * MON-FRI` | Weekdays at 9 AM |
| `0 0 0 1 * ?` | First day of every month |

### Settings

- `MaintenanceCheckCron`: Cron expression for maintenance check job schedule
- `MaintenanceDaysThreshold`: Number of days ahead to check for upcoming maintenance

---

## Maintenance Settings

Configure maintenance calculation parameters.

**appsettings.json:**

```json
{
  "Maintenance": {
    "DefaultIntervalDays": 90,
    "AlertThresholdDays": 3
  }
}
```

**Settings:**
- `DefaultIntervalDays`: Default number of days between maintenance (90 days = ~3 months)
- `AlertThresholdDays`: Number of days before maintenance to trigger alerts

---

## Environment Variables

### Setting Environment Variables

**Windows (PowerShell):**

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__DefaultConnection="Server=prod-server;Database=FleetManager;..."
```

**Linux/macOS:**

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=FleetManager;..."
```

**Docker Compose:**

```yaml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=FleetManager;...
```

### Common Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Application environment | Development, Production |
| `ConnectionStrings__DefaultConnection` | SQL Server connection | Server=...;Database=... |
| `ConnectionStrings__Redis` | Redis connection | localhost:6379 |
| `SQL_PASSWORD` | SQL Server password | YourStrong@Passw0rd |
| `SEQ_API_KEY` | Seq API key | AbCdEf123456 |
| `Quartz__MaintenanceCheckCron` | Job schedule | 0 0 0 * * ? |
| `Maintenance__DefaultIntervalDays` | Maintenance interval | 90 |

### Hierarchical Configuration

Use double underscores (`__`) to represent nested configuration:

```bash
# appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}

# Environment variable
ConnectionStrings__DefaultConnection="..."
```

---

## Configuration Priority

Configuration sources are loaded in the following order (later sources override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User secrets (Development only)
4. Environment variables
5. Command-line arguments

---

## User Secrets (Development)

For sensitive data in development, use User Secrets instead of committing to source control.

### Initialize User Secrets

```bash
dotnet user-secrets init --project src/FleetManager.Api
```

### Set Secrets

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;..." --project src/FleetManager.Api
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379" --project src/FleetManager.Api
```

### List Secrets

```bash
dotnet user-secrets list --project src/FleetManager.Api
```

### Remove Secrets

```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection" --project src/FleetManager.Api
```

### Clear All Secrets

```bash
dotnet user-secrets clear --project src/FleetManager.Api
```

---

## Validation

### Validate Configuration on Startup

Add configuration validation in `Program.cs`:

```csharp
builder.Services.AddOptions<CacheSettings>()
    .Bind(builder.Configuration.GetSection("CacheSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

public class CacheSettings
{
    [Range(1, 1440)]
    public int DefaultExpirationMinutes { get; set; }
    
    [Range(1, 60)]
    public int AvailableVehiclesCacheMinutes { get; set; }
    
    [Range(1, 60)]
    public int AvailableDriversCacheMinutes { get; set; }
}
```

---

## Troubleshooting

### Configuration Not Loading

1. Check file names match exactly (case-sensitive on Linux)
2. Verify `ASPNETCORE_ENVIRONMENT` is set correctly
3. Check file is set to "Copy to Output Directory"

### Environment Variables Not Working

1. Verify double underscore (`__`) syntax
2. Restart application after setting variables
3. Check variable is accessible: `echo $env:VARIABLE_NAME` (PowerShell)

### Connection String Issues

1. Verify server is accessible
2. Check credentials are correct
3. Test connection with SQL client tool
4. Ensure firewall allows connection

### Logging Not Working

1. Check log directory has write permissions
2. Verify Seq server is running
3. Check log level is not too restrictive
4. Review Serilog configuration syntax
