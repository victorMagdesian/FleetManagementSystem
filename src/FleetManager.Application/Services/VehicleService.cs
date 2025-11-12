using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FleetManager.Application.Services;

/// <summary>
/// Service implementation for vehicle management operations.
/// </summary>
public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService? _cacheService;
    private readonly ILogger<VehicleService> _logger;
    private const string AvailableVehiclesCacheKey = "vehicles:available";

    public VehicleService(
        IVehicleRepository vehicleRepository, 
        IMapper mapper, 
        ILogger<VehicleService> logger,
        ICacheService? cacheService = null)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<VehicleResponse> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting vehicle by ID: {VehicleId}", id);
        
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            _logger.LogWarning("Vehicle not found: {VehicleId}", id);
            throw new EntityNotFoundException("Vehicle", id);
        }

        _logger.LogDebug("Vehicle retrieved successfully: {VehicleId}, Plate: {Plate}", id, vehicle.Plate);
        return _mapper.Map<VehicleResponse>(vehicle);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
    {
        _logger.LogDebug("Getting all vehicles");
        
        var vehicles = await _vehicleRepository.GetAllAsync();
        var vehicleList = vehicles.ToList();
        
        _logger.LogInformation("Retrieved {Count} vehicles", vehicleList.Count);
        return _mapper.Map<IEnumerable<VehicleResponse>>(vehicleList);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleResponse>> GetAvailableAsync()
    {
        _logger.LogDebug("Getting available vehicles");
        
        // Try to get from cache if caching is enabled
        if (_cacheService != null)
        {
            var cachedVehicles = await _cacheService.GetAsync<IEnumerable<VehicleResponse>>(AvailableVehiclesCacheKey);
            if (cachedVehicles != null)
            {
                _logger.LogDebug("Available vehicles retrieved from cache");
                return cachedVehicles;
            }
        }

        // Get from repository
        var vehicles = await _vehicleRepository.GetAvailableAsync();
        var vehicleList = vehicles.ToList();
        var result = _mapper.Map<IEnumerable<VehicleResponse>>(vehicleList);

        _logger.LogInformation("Retrieved {Count} available vehicles", vehicleList.Count);

        // Cache the result if caching is enabled
        if (_cacheService != null)
        {
            await _cacheService.SetAsync(AvailableVehiclesCacheKey, result, 5);
            _logger.LogDebug("Available vehicles cached");
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleResponse>> GetUpcomingMaintenanceAsync(int daysThreshold)
    {
        _logger.LogDebug("Getting vehicles with upcoming maintenance within {DaysThreshold} days", daysThreshold);
        
        if (daysThreshold < 0)
        {
            _logger.LogWarning("Invalid days threshold: {DaysThreshold}", daysThreshold);
            throw new ArgumentException("Days threshold cannot be negative", nameof(daysThreshold));
        }

        var vehicles = await _vehicleRepository.GetVehiclesWithUpcomingMaintenanceAsync(daysThreshold);
        var vehicleList = vehicles.ToList();
        
        _logger.LogInformation("Found {Count} vehicles with upcoming maintenance within {DaysThreshold} days", 
            vehicleList.Count, daysThreshold);
        
        return _mapper.Map<IEnumerable<VehicleResponse>>(vehicleList);
    }

    /// <inheritdoc />
    public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Creating new vehicle with plate: {Plate}", request.Plate);

        // Validate plate uniqueness
        var existingVehicle = await _vehicleRepository.GetByPlateAsync(request.Plate);
        if (existingVehicle != null)
        {
            _logger.LogWarning("Duplicate vehicle plate detected: {Plate}", request.Plate);
            throw new DuplicateEntityException("Vehicle", "Plate", request.Plate);
        }

        // Create new vehicle entity
        var vehicle = _mapper.Map<Vehicle>(request);
        
        // Add to repository
        await _vehicleRepository.AddAsync(vehicle);

        _logger.LogInformation("Vehicle created successfully: {VehicleId}, Plate: {Plate}", vehicle.Id, vehicle.Plate);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableVehiclesCacheKey);
            _logger.LogDebug("Available vehicles cache invalidated");
        }

        return _mapper.Map<VehicleResponse>(vehicle);
    }

    /// <inheritdoc />
    public async Task<VehicleResponse> UpdateAsync(Guid id, UpdateVehicleRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Updating vehicle: {VehicleId}", id);

        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            _logger.LogWarning("Vehicle not found for update: {VehicleId}", id);
            throw new EntityNotFoundException("Vehicle", id);
        }

        // Update vehicle properties using reflection to access private setters
        var vehicleType = typeof(Vehicle);
        
        var modelProperty = vehicleType.GetProperty(nameof(Vehicle.Model));
        modelProperty?.SetValue(vehicle, request.Model);
        
        var yearProperty = vehicleType.GetProperty(nameof(Vehicle.Year));
        yearProperty?.SetValue(vehicle, request.Year);
        
        // Update mileage using the public method
        vehicle.UpdateMileage(request.Mileage);

        await _vehicleRepository.UpdateAsync(vehicle);

        _logger.LogInformation("Vehicle updated successfully: {VehicleId}, Plate: {Plate}", id, vehicle.Plate);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableVehiclesCacheKey);
            _logger.LogDebug("Available vehicles cache invalidated");
        }

        return _mapper.Map<VehicleResponse>(vehicle);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting vehicle: {VehicleId}", id);
        
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            _logger.LogWarning("Vehicle not found for deletion: {VehicleId}", id);
            throw new EntityNotFoundException("Vehicle", id);
        }

        await _vehicleRepository.DeleteAsync(id);

        _logger.LogInformation("Vehicle deleted successfully: {VehicleId}, Plate: {Plate}", id, vehicle.Plate);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableVehiclesCacheKey);
            _logger.LogDebug("Available vehicles cache invalidated");
        }
    }
}
