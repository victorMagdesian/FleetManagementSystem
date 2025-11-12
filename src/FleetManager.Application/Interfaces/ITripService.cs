using FleetManager.Application.DTOs;

namespace FleetManager.Application.Interfaces;

/// <summary>
/// Service interface for trip management operations.
/// Handles trip lifecycle including starting, ending, and querying trips.
/// </summary>
public interface ITripService
{
    /// <summary>
    /// Starts a new trip with the specified vehicle and driver.
    /// Validates vehicle and driver availability before creating the trip.
    /// </summary>
    /// <param name="request">Trip start request containing vehicle, driver, and route information</param>
    /// <returns>The created trip response</returns>
    /// <exception cref="EntityNotFoundException">Thrown when vehicle or driver is not found</exception>
    /// <exception cref="InvalidOperationException">Thrown when vehicle or driver is not available</exception>
    Task<TripResponse> StartTripAsync(StartTripRequest request);

    /// <summary>
    /// Ends an active trip by setting the end date, distance, and updating vehicle status.
    /// </summary>
    /// <param name="tripId">The unique identifier of the trip to end</param>
    /// <param name="request">Trip end request containing distance traveled</param>
    /// <returns>The updated trip response</returns>
    /// <exception cref="EntityNotFoundException">Thrown when trip is not found</exception>
    /// <exception cref="InvalidOperationException">Thrown when trip is already ended</exception>
    Task<TripResponse> EndTripAsync(Guid tripId, EndTripRequest request);

    /// <summary>
    /// Retrieves a trip by its unique identifier.
    /// </summary>
    /// <param name="id">The trip's unique identifier</param>
    /// <returns>The trip response</returns>
    /// <exception cref="EntityNotFoundException">Thrown when trip is not found</exception>
    Task<TripResponse> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all trips in the system.
    /// </summary>
    /// <returns>Collection of all trip responses</returns>
    Task<IEnumerable<TripResponse>> GetAllAsync();

    /// <summary>
    /// Retrieves all active trips (trips that have not ended yet).
    /// </summary>
    /// <returns>Collection of active trip responses</returns>
    Task<IEnumerable<TripResponse>> GetActiveTripsAsync();
}
