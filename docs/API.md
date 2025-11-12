# API Documentation

Complete REST API documentation for FleetManager.

## Base URL

```
http://localhost:5000/api
```

## Table of Contents

- [Vehicles API](#vehicles-api)
- [Drivers API](#drivers-api)
- [Maintenance API](#maintenance-api)
- [Trips API](#trips-api)
- [Error Responses](#error-responses)

---

## Vehicles API

### Get All Vehicles

Retrieve a list of all vehicles in the fleet.

**Endpoint:** `GET /api/vehicles`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "plate": "ABC1234",
    "model": "Toyota Corolla",
    "year": 2023,
    "mileage": 15000,
    "lastMaintenanceDate": "2024-10-01T00:00:00",
    "nextMaintenanceDate": "2024-12-30T00:00:00",
    "status": "Available"
  }
]
```

### Get Vehicle by ID

Retrieve a specific vehicle by its ID.

**Endpoint:** `GET /api/vehicles/{id}`

**Parameters:**
- `id` (path, required): Vehicle GUID

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "plate": "ABC1234",
  "model": "Toyota Corolla",
  "year": 2023,
  "mileage": 15000,
  "lastMaintenanceDate": "2024-10-01T00:00:00",
  "nextMaintenanceDate": "2024-12-30T00:00:00",
  "status": "Available"
}
```

**Error Response:** `404 Not Found`

```json
{
  "error": "Vehicle with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
}
```

### Get Available Vehicles

Retrieve all vehicles with status "Available" (cached for 5 minutes).

**Endpoint:** `GET /api/vehicles/available`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "plate": "ABC1234",
    "model": "Toyota Corolla",
    "year": 2023,
    "mileage": 15000,
    "lastMaintenanceDate": "2024-10-01T00:00:00",
    "nextMaintenanceDate": "2024-12-30T00:00:00",
    "status": "Available"
  }
]
```

### Create Vehicle

Create a new vehicle in the fleet.

**Endpoint:** `POST /api/vehicles`

**Request Body:**

```json
{
  "plate": "ABC1234",
  "model": "Toyota Corolla",
  "year": 2023,
  "mileage": 0
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "plate": "ABC1234",
  "model": "Toyota Corolla",
  "year": 2023,
  "mileage": 0,
  "lastMaintenanceDate": "2024-11-12T00:00:00",
  "nextMaintenanceDate": "2025-02-10T00:00:00",
  "status": "Available"
}
```

**Error Response:** `409 Conflict`

```json
{
  "error": "Vehicle with Plate 'ABC1234' already exists"
}
```

### Update Vehicle

Update an existing vehicle.

**Endpoint:** `PUT /api/vehicles/{id}`

**Parameters:**
- `id` (path, required): Vehicle GUID

**Request Body:**

```json
{
  "model": "Toyota Corolla XEI",
  "year": 2023,
  "mileage": 15500
}
```

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "plate": "ABC1234",
  "model": "Toyota Corolla XEI",
  "year": 2023,
  "mileage": 15500,
  "lastMaintenanceDate": "2024-10-01T00:00:00",
  "nextMaintenanceDate": "2024-12-30T00:00:00",
  "status": "Available"
}
```

### Delete Vehicle

Delete a vehicle from the fleet.

**Endpoint:** `DELETE /api/vehicles/{id}`

**Parameters:**
- `id` (path, required): Vehicle GUID

**Response:** `204 No Content`

---

## Drivers API

### Get All Drivers

Retrieve a list of all drivers.

**Endpoint:** `GET /api/drivers`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "licenseNumber": "12345678900",
    "phone": "+5511999999999",
    "active": true
  }
]
```

### Get Driver by ID

Retrieve a specific driver by ID.

**Endpoint:** `GET /api/drivers/{id}`

**Parameters:**
- `id` (path, required): Driver GUID

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "licenseNumber": "12345678900",
  "phone": "+5511999999999",
  "active": true
}
```

### Get Available Drivers

Retrieve all active drivers (cached for 5 minutes).

**Endpoint:** `GET /api/drivers/available`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "licenseNumber": "12345678900",
    "phone": "+5511999999999",
    "active": true
  }
]
```

### Create Driver

Create a new driver.

**Endpoint:** `POST /api/drivers`

**Request Body:**

```json
{
  "name": "João Silva",
  "licenseNumber": "12345678900",
  "phone": "+5511999999999"
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "licenseNumber": "12345678900",
  "phone": "+5511999999999",
  "active": true
}
```

**Error Response:** `409 Conflict`

```json
{
  "error": "Driver with LicenseNumber '12345678900' already exists"
}
```

### Update Driver

Update an existing driver.

**Endpoint:** `PUT /api/drivers/{id}`

**Parameters:**
- `id` (path, required): Driver GUID

**Request Body:**

```json
{
  "name": "João Silva Santos",
  "licenseNumber": "12345678900",
  "phone": "+5511988888888"
}
```

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva Santos",
  "licenseNumber": "12345678900",
  "phone": "+5511988888888",
  "active": true
}
```

### Delete Driver

Delete a driver.

**Endpoint:** `DELETE /api/drivers/{id}`

**Parameters:**
- `id` (path, required): Driver GUID

**Response:** `204 No Content`

### Activate Driver

Activate a deactivated driver.

**Endpoint:** `POST /api/drivers/{id}/activate`

**Parameters:**
- `id` (path, required): Driver GUID

**Response:** `204 No Content`

### Deactivate Driver

Deactivate an active driver.

**Endpoint:** `POST /api/drivers/{id}/deactivate`

**Parameters:**
- `id` (path, required): Driver GUID

**Response:** `204 No Content`

---

## Maintenance API

### Get Upcoming Maintenance

Retrieve vehicles with maintenance due within a specified number of days.

**Endpoint:** `GET /api/maintenance/upcoming`

**Query Parameters:**
- `days` (optional, default: 3): Number of days threshold

**Example:** `GET /api/maintenance/upcoming?days=7`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "plate": "ABC1234",
    "model": "Toyota Corolla",
    "year": 2023,
    "mileage": 15000,
    "lastMaintenanceDate": "2024-10-01T00:00:00",
    "nextMaintenanceDate": "2024-11-15T00:00:00",
    "status": "Available"
  }
]
```

### Create Maintenance Record

Create a new maintenance record for a vehicle.

**Endpoint:** `POST /api/maintenance`

**Request Body:**

```json
{
  "vehicleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2024-11-12T10:00:00",
  "description": "Troca de óleo e filtros",
  "cost": 350.00
}
```

**Response:** `201 Created`

```json
{
  "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
  "vehicleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2024-11-12T10:00:00",
  "description": "Troca de óleo e filtros",
  "cost": 350.00
}
```

**Side Effects:**
- Updates vehicle's `lastMaintenanceDate`
- Recalculates vehicle's `nextMaintenanceDate`
- Changes vehicle status to `InMaintenance`

### Get Maintenance History

Retrieve all maintenance records for a specific vehicle.

**Endpoint:** `GET /api/maintenance/vehicle/{vehicleId}`

**Parameters:**
- `vehicleId` (path, required): Vehicle GUID

**Response:** `200 OK`

```json
[
  {
    "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
    "vehicleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "date": "2024-11-12T10:00:00",
    "description": "Troca de óleo e filtros",
    "cost": 350.00
  },
  {
    "id": "8fa85f64-5717-4562-b3fc-2c963f66afa8",
    "vehicleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "date": "2024-08-15T14:30:00",
    "description": "Revisão completa",
    "cost": 1200.00
  }
]
```

---

## Trips API

### Get All Trips

Retrieve a list of all trips.

**Endpoint:** `GET /api/trips`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "route": "São Paulo - Rio de Janeiro",
    "startDate": "2024-11-12T08:00:00",
    "endDate": "2024-11-12T14:00:00",
    "distance": 450
  }
]
```

### Get Trip by ID

Retrieve a specific trip by ID.

**Endpoint:** `GET /api/trips/{id}`

**Parameters:**
- `id` (path, required): Trip GUID

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
  "route": "São Paulo - Rio de Janeiro",
  "startDate": "2024-11-12T08:00:00",
  "endDate": "2024-11-12T14:00:00",
  "distance": 450
}
```

### Get Active Trips

Retrieve all trips that are currently in progress (no end date).

**Endpoint:** `GET /api/trips/active`

**Response:** `200 OK`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "route": "São Paulo - Campinas",
    "startDate": "2024-11-12T10:00:00",
    "endDate": null,
    "distance": 0
  }
]
```

### Start Trip

Start a new trip with a vehicle and driver.

**Endpoint:** `POST /api/trips/start`

**Request Body:**

```json
{
  "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
  "route": "São Paulo - Rio de Janeiro"
}
```

**Response:** `201 Created`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
  "route": "São Paulo - Rio de Janeiro",
  "startDate": "2024-11-12T10:00:00",
  "endDate": null,
  "distance": 0
}
```

**Side Effects:**
- Changes vehicle status to `InUse`
- Validates vehicle is available
- Validates driver is active

**Error Response:** `400 Bad Request`

```json
{
  "error": "Vehicle is not available for trip"
}
```

### End Trip

End an active trip and update vehicle mileage.

**Endpoint:** `POST /api/trips/end/{id}`

**Parameters:**
- `id` (path, required): Trip GUID

**Request Body:**

```json
{
  "distance": 450
}
```

**Response:** `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
  "route": "São Paulo - Rio de Janeiro",
  "startDate": "2024-11-12T08:00:00",
  "endDate": "2024-11-12T14:00:00",
  "distance": 450
}
```

**Side Effects:**
- Sets trip `endDate` to current time
- Updates trip `distance`
- Increments vehicle `mileage` by distance
- Changes vehicle status back to `Available`

---

## Error Responses

All error responses follow a consistent format:

### 400 Bad Request

Invalid request data or business rule violation.

```json
{
  "error": "Vehicle is not available for trip"
}
```

### 404 Not Found

Requested resource does not exist.

```json
{
  "error": "Vehicle with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
}
```

### 409 Conflict

Resource already exists (duplicate).

```json
{
  "error": "Vehicle with Plate 'ABC1234' already exists"
}
```

### 500 Internal Server Error

Unexpected server error.

```json
{
  "error": "An internal error occurred"
}
```

## HTTP Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 OK | Request successful |
| 201 Created | Resource created successfully |
| 204 No Content | Request successful, no content to return |
| 400 Bad Request | Invalid request or business rule violation |
| 404 Not Found | Resource not found |
| 409 Conflict | Resource already exists |
| 500 Internal Server Error | Unexpected server error |

## Testing with cURL

### Create a Vehicle

```bash
curl -X POST http://localhost:5000/api/vehicles \
  -H "Content-Type: application/json" \
  -d '{
    "plate": "ABC1234",
    "model": "Toyota Corolla",
    "year": 2023,
    "mileage": 0
  }'
```

### Get Available Vehicles

```bash
curl http://localhost:5000/api/vehicles/available
```

### Start a Trip

```bash
curl -X POST http://localhost:5000/api/trips/start \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "driverId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "route": "São Paulo - Rio de Janeiro"
  }'
```

## Swagger UI

For interactive API documentation and testing, access:

```
http://localhost:5000/swagger
```

The Swagger UI provides:
- Complete API documentation
- Interactive request/response testing
- Schema definitions
- Example values
