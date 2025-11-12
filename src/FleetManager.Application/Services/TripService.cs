using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FleetManager.Application.Services;

/// <summary>
/// Service implementation for trip management operations.
/// </summary>
public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TripService> _logger;

    public TripService(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IMapper mapper,
        ILogger<TripService> logger)
    {
        _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TripResponse> StartTripAsync(StartTripRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Starting trip for vehicle: {VehicleId}, driver: {DriverId}, route: {Route}", 
            request.VehicleId, request.DriverId, request.Route);

        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            _logger.LogWarning("Vehicle not found for trip: {VehicleId}", request.VehicleId);
            throw new EntityNotFoundException("Vehicle", request.VehicleId);
        }

        // Validate driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId);
        if (driver == null)
        {
            _logger.LogWarning("Driver not found for trip: {DriverId}", request.DriverId);
            throw new EntityNotFoundException("Driver", request.DriverId);
        }

        // Validate vehicle availability
        if (vehicle.Status != VehicleStatus.Available)
        {
            _logger.LogWarning("Vehicle not available: {Plate}, Status: {Status}", vehicle.Plate, vehicle.Status);
            throw new Exceptions.InvalidOperationException(
                $"Vehicle {vehicle.Plate} is not available. Current status: {vehicle.Status}");
        }

        // Validate driver availability
        if (!driver.Active)
        {
            _logger.LogWarning("Driver not active: {Name}", driver.Name);
            throw new Exceptions.InvalidOperationException(
                $"Driver {driver.Name} is not active");
        }

        // Check if driver has any active trips
        var activeTrips = await _tripRepository.GetActiveTripsAsync();
        var driverHasActiveTrip = activeTrips.Any(t => t.DriverId == request.DriverId);
        if (driverHasActiveTrip)
        {
            _logger.LogWarning("Driver already on active trip: {Name}", driver.Name);
            throw new Exceptions.InvalidOperationException(
                $"Driver {driver.Name} is already on an active trip");
        }

        // Create new trip
        var trip = new Trip(request.VehicleId, request.DriverId, request.Route, DateTime.UtcNow);

        // Update vehicle status to InUse
        vehicle.StartTrip();

        // Save changes
        await _tripRepository.AddAsync(trip);
        await _vehicleRepository.UpdateAsync(vehicle);

        _logger.LogInformation("Trip started successfully: {TripId}, Vehicle: {Plate}, Driver: {DriverName}", 
            trip.Id, vehicle.Plate, driver.Name);

        return _mapper.Map<TripResponse>(trip);
    }

    /// <inheritdoc />
    public async Task<TripResponse> EndTripAsync(Guid tripId, EndTripRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Ending trip: {TripId}, Distance: {Distance} km", tripId, request.Distance);

        // Validate trip exists
        var trip = await _tripRepository.GetByIdAsync(tripId);
        if (trip == null)
        {
            _logger.LogWarning("Trip not found: {TripId}", tripId);
            throw new EntityNotFoundException("Trip", tripId);
        }

        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(trip.VehicleId);
        if (vehicle == null)
        {
            _logger.LogWarning("Vehicle not found for trip: {VehicleId}", trip.VehicleId);
            throw new EntityNotFoundException("Vehicle", trip.VehicleId);
        }

        // End the trip (this will set EndDate and Distance)
        trip.End(request.Distance);

        // Update vehicle status to Available and update mileage
        vehicle.EndTrip(request.Distance);

        // Save changes
        await _tripRepository.UpdateAsync(trip);
        await _vehicleRepository.UpdateAsync(vehicle);

        _logger.LogInformation("Trip ended successfully: {TripId}, Vehicle: {Plate}, Distance: {Distance} km, New mileage: {Mileage} km", 
            tripId, vehicle.Plate, request.Distance, vehicle.Mileage);

        return _mapper.Map<TripResponse>(trip);
    }

    /// <inheritdoc />
    public async Task<TripResponse> GetByIdAsync(Guid id)
    {
        var trip = await _tripRepository.GetByIdAsync(id);

        if (trip == null)
        {
            throw new EntityNotFoundException("Trip", id);
        }

        return _mapper.Map<TripResponse>(trip);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TripResponse>> GetAllAsync()
    {
        var trips = await _tripRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TripResponse>>(trips);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TripResponse>> GetActiveTripsAsync()
    {
        var trips = await _tripRepository.GetActiveTripsAsync();
        return _mapper.Map<IEnumerable<TripResponse>>(trips);
    }
}
