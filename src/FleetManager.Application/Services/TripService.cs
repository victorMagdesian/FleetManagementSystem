using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using FleetManager.Domain.Interfaces;

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

    public TripService(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IMapper mapper)
    {
        _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<TripResponse> StartTripAsync(StartTripRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", request.VehicleId);
        }

        // Validate driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId);
        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", request.DriverId);
        }

        // Validate vehicle availability
        if (vehicle.Status != VehicleStatus.Available)
        {
            throw new Exceptions.InvalidOperationException(
                $"Vehicle {vehicle.Plate} is not available. Current status: {vehicle.Status}");
        }

        // Validate driver availability
        if (!driver.Active)
        {
            throw new Exceptions.InvalidOperationException(
                $"Driver {driver.Name} is not active");
        }

        // Check if driver has any active trips
        var activeTrips = await _tripRepository.GetActiveTripsAsync();
        var driverHasActiveTrip = activeTrips.Any(t => t.DriverId == request.DriverId);
        if (driverHasActiveTrip)
        {
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

        return _mapper.Map<TripResponse>(trip);
    }

    /// <inheritdoc />
    public async Task<TripResponse> EndTripAsync(Guid tripId, EndTripRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validate trip exists
        var trip = await _tripRepository.GetByIdAsync(tripId);
        if (trip == null)
        {
            throw new EntityNotFoundException("Trip", tripId);
        }

        // Validate vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(trip.VehicleId);
        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", trip.VehicleId);
        }

        // End the trip (this will set EndDate and Distance)
        trip.End(request.Distance);

        // Update vehicle status to Available and update mileage
        vehicle.EndTrip(request.Distance);

        // Save changes
        await _tripRepository.UpdateAsync(trip);
        await _vehicleRepository.UpdateAsync(vehicle);

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
