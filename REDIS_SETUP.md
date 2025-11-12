# Redis Caching Setup Guide

## Quick Start

### 1. Start Docker Containers

```bash
docker-compose up -d
```

This will start both SQL Server and Redis containers.

### 2. Verify Containers are Running

```bash
docker-compose ps
```

You should see both `fleetmanager-sqlserver` and `fleetmanager-redis` running.

### 3. Test Redis Connection

```bash
docker exec -it fleetmanager-redis redis-cli ping
```

Expected output: `PONG`

### 4. Run the Application

```bash
dotnet run --project src/FleetManager.Api
```

## Testing Cache Behavior

### Monitor Redis Keys

In a separate terminal, monitor Redis operations:

```bash
docker exec -it fleetmanager-redis redis-cli MONITOR
```

### Test Available Vehicles Caching

1. First request (cache miss):
```bash
curl http://localhost:5000/api/vehicles/available
```

2. Second request (cache hit - should be faster):
```bash
curl http://localhost:5000/api/vehicles/available
```

3. Update a vehicle to invalidate cache:
```bash
curl -X PUT http://localhost:5000/api/vehicles/{id} -H "Content-Type: application/json" -d "{...}"
```

4. Request again (cache miss after invalidation):
```bash
curl http://localhost:5000/api/vehicles/available
```

### Check Cached Keys

```bash
docker exec -it fleetmanager-redis redis-cli KEYS "*"
```

Expected keys:
- `vehicles:available`
- `drivers:available`

### View Cache Content

```bash
docker exec -it fleetmanager-redis redis-cli GET "vehicles:available"
```

### Check Cache TTL

```bash
docker exec -it fleetmanager-redis redis-cli TTL "vehicles:available"
```

## Cache Configuration

Cache settings are configured in `appsettings.json`:

```json
"CacheSettings": {
  "DefaultExpirationMinutes": 10,
  "AvailableVehiclesCacheMinutes": 5,
  "AvailableDriversCacheMinutes": 5
}
```

## Cache Invalidation

Cache is automatically invalidated when:
- Creating a new vehicle or driver
- Updating a vehicle or driver
- Deleting a vehicle or driver
- Activating or deactivating a driver

## Troubleshooting

### Redis Connection Failed

If the application can't connect to Redis, it will continue to work without caching. Check:

1. Redis container is running:
```bash
docker-compose ps
```

2. Redis connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "Redis": "localhost:6379"
}
```

### Clear All Cache

```bash
docker exec -it fleetmanager-redis redis-cli FLUSHALL
```

### Stop Containers

```bash
docker-compose down
```

### Stop and Remove All Data

```bash
docker-compose down -v
```
