using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;

namespace FleetManager.Application.Services;

/// <summary>
/// Service implementation for vehicle management operations.
/// </summary>
public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService? _cacheService;
    private const string AvailableVehiclesCacheKey = "vehicles:available";

    public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper, ICacheService? cacheService = null)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<VehicleResponse> GetByIdAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", id);
        }

        return _mapper.Map<VehicleResponse>(vehicle);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleResponse>> GetAvailableAsync()
    {
        // Try to get from cache if caching is enabled
        if (_cacheService != null)
        {
            var cachedVehicles = await _cacheService.GetAsync<IEnumerable<VehicleResponse>>(AvailableVehiclesCacheKey);
            if (cachedVehicles != null)
            {
                return cachedVehicles;
            }
        }

        // Get from repository
        var vehicles = await _vehicleRepository.GetAvailableAsync();
        var result = _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);

        // Cache the result if caching is enabled
        if (_cacheService != null)
        {
            await _cacheService.SetAsync(AvailableVehiclesCacheKey, result, 5);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VehicleResponse>> GetUpcomingMaintenanceAsync(int daysThreshold)
    {
        if (daysThreshold < 0)
        {
            throw new ArgumentException("Days threshold cannot be negative", nameof(daysThreshold));
        }

        var vehicles = await _vehicleRepository.GetVehiclesWithUpcomingMaintenanceAsync(daysThreshold);
        return _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);
    }

    /// <inheritdoc />
    public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validate plate uniqueness
        var existingVehicle = await _vehicleRepository.GetByPlateAsync(request.Plate);
        if (existingVehicle != null)
        {
            throw new DuplicateEntityException("Vehicle", "Plate", request.Plate);
        }

        // Create new vehicle entity
        var vehicle = _mapper.Map<Vehicle>(request);
        
        // Add to repository
        await _vehicleRepository.AddAsync(vehicle);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableVehiclesCacheKey);
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

        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
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

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableVehiclesCacheKey);
        }

        return _mapper.Map<VehicleResponse>(vehicle);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        
        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", id);
        }

        await _vehicleRepository.DeleteAsync(id);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableVehiclesCacheKey);
        }
    }
}
