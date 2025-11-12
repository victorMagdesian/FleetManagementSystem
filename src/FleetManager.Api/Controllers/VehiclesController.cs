using FleetManager.Application.DTOs;
using FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Api.Controllers;

/// <summary>
/// Controller for managing vehicle operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all vehicles in the fleet.
    /// </summary>
    /// <returns>List of all vehicles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetAll()
    {
        _logger.LogInformation("Getting all vehicles");
        var vehicles = await _vehicleService.GetAllAsync();
        return Ok(vehicles);
    }

    /// <summary>
    /// Gets a specific vehicle by ID.
    /// </summary>
    /// <param name="id">The vehicle ID</param>
    /// <returns>The vehicle details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponse>> GetById(Guid id)
    {
        _logger.LogInformation("Getting vehicle with ID: {VehicleId}", id);
        var vehicle = await _vehicleService.GetByIdAsync(id);
        return Ok(vehicle);
    }

    /// <summary>
    /// Creates a new vehicle.
    /// </summary>
    /// <param name="request">The vehicle creation request</param>
    /// <returns>The created vehicle</returns>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VehicleResponse>> Create([FromBody] CreateVehicleRequest request)
    {
        _logger.LogInformation("Creating new vehicle with plate: {Plate}", request.Plate);
        var vehicle = await _vehicleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// Updates an existing vehicle.
    /// </summary>
    /// <param name="id">The vehicle ID</param>
    /// <param name="request">The vehicle update request</param>
    /// <returns>The updated vehicle</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleResponse>> Update(Guid id, [FromBody] UpdateVehicleRequest request)
    {
        _logger.LogInformation("Updating vehicle with ID: {VehicleId}", id);
        var vehicle = await _vehicleService.UpdateAsync(id, request);
        return Ok(vehicle);
    }

    /// <summary>
    /// Deletes a vehicle.
    /// </summary>
    /// <param name="id">The vehicle ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting vehicle with ID: {VehicleId}", id);
        await _vehicleService.DeleteAsync(id);
        return NoContent();
    }
}
