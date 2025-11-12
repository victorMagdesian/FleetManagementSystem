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

    public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        var vehicles = await _vehicleRepository.GetAvailableAsync();
        return _mapper.Map<IEnumerable<VehicleResponse>>(vehicles);
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
    }
}
