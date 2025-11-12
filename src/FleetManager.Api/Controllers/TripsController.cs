using FleetManager.Application.DTOs;
using FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Api.Controllers;

/// <summary>
/// Controller for managing trip operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ILogger<TripsController> _logger;

    public TripsController(ITripService tripService, ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all trips in the system.
    /// </summary>
    /// <returns>List of all trips</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripResponse>>> GetAll()
    {
        _logger.LogInformation("Getting all trips");
        var trips = await _tripService.GetAllAsync();
        return Ok(trips);
    }

    /// <summary>
    /// Gets a specific trip by ID.
    /// </summary>
    /// <param name="id">The trip ID</param>
    /// <returns>The trip details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripResponse>> GetById(Guid id)
    {
        _logger.LogInformation("Getting trip with ID: {TripId}", id);
        var trip = await _tripService.GetByIdAsync(id);
        return Ok(trip);
    }

    /// <summary>
    /// Gets all active trips (trips that have not ended yet).
    /// </summary>
    /// <returns>List of active trips</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<TripResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TripResponse>>> GetActive()
    {
        _logger.LogInformation("Getting active trips");
        var trips = await _tripService.GetActiveTripsAsync();
        return Ok(trips);
    }

    /// <summary>
    /// Starts a new trip with the specified vehicle and driver.
    /// </summary>
    /// <param name="request">The trip start request</param>
    /// <returns>The created trip</returns>
    [HttpPost("start")]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripResponse>> Start([FromBody] StartTripRequest request)
    {
        _logger.LogInformation("Starting trip for vehicle: {VehicleId} and driver: {DriverId}", 
            request.VehicleId, request.DriverId);
        var trip = await _tripService.StartTripAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
    }

    /// <summary>
    /// Ends an active trip.
    /// </summary>
    /// <param name="id">The trip ID</param>
    /// <param name="request">The trip end request</param>
    /// <returns>The updated trip</returns>
    [HttpPost("end/{id}")]
    [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripResponse>> End(Guid id, [FromBody] EndTripRequest request)
    {
        _logger.LogInformation("Ending trip with ID: {TripId}", id);
        var trip = await _tripService.EndTripAsync(id, request);
        return Ok(trip);
    }
}
