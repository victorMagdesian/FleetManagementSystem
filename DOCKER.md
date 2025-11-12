# Docker Configuration for FleetManager

This document describes how to run the FleetManager application using Docker Compose.

## Services

The docker-compose.yml file defines the following services:

- **api**: FleetManager.Api application (ASP.NET Core 8.0)
- **sqlserver**: Microsoft SQL Server 2022 database
- **redis**: Redis cache server
- **seq**: Seq log aggregation server

## Prerequisites

- Docker Desktop installed and running
- .NET 8.0 SDK (for building the application)

## Quick Start

1. Build and start all services:
```bash
docker-compose up -d
```

2. Check service status:
```bash
docker-compose ps
```

3. View logs:
```bash
docker-compose logs -f api
```

4. Stop all services:
```bash
docker-compose down
```

## Service URLs

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433
- **Redis**: localhost:6379
- **Seq**: http://localhost:5341

## Environment Variables

The API service uses the following environment variables (configured in docker-compose.yml):

- `ASPNETCORE_ENVIRONMENT`: Development
- `ConnectionStrings__DefaultConnection`: SQL Server connection string
- `Redis__Configuration`: Redis connection string
- `Redis__InstanceName`: Redis key prefix
- `Quartz__MaintenanceCheckCron`: Cron expression for maintenance checks
- `Quartz__MaintenanceDaysThreshold`: Days threshold for maintenance alerts
- `Serilog__WriteTo__1__Name`: Seq logging configuration
- `Serilog__WriteTo__1__Args__serverUrl`: Seq server URL
- `Maintenance__DefaultIntervalDays`: Default maintenance interval

## Database Initialization

After starting the services for the first time, you need to initialize the database:

1. Run migrations:
```bash
docker exec -it fleetmanager-api dotnet ef database update
```

Or from your local machine (if you have .NET SDK installed):
```bash
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

## Data Persistence

The following volumes are created for data persistence:

- `sqlserver-data`: SQL Server database files
- `redis-data`: Redis persistence files
- `seq-data`: Seq logs and configuration

## Troubleshooting

### Services not starting

Check the logs for each service:
```bash
docker-compose logs sqlserver
docker-compose logs redis
docker-compose logs seq
docker-compose logs api
```

### Database connection issues

Ensure SQL Server is healthy:
```bash
docker ps
```

The STATUS column should show "healthy" for the sqlserver container.

### Rebuilding the API image

If you make changes to the code:
```bash
docker-compose build api
docker-compose up -d --force-recreate api
```

## Development

For development, you can run the API locally and use only the infrastructure services:

```bash
# Start only infrastructure services
docker-compose up -d sqlserver redis seq

# Run the API locally
cd src/FleetManager.Api
dotnet run
```
