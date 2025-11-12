using FleetManager.Application.DTOs;
using FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Api.Controllers;

/// <summary>
/// Controller for managing driver operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly ILogger<DriversController> _logger;

    public DriversController(IDriverService driverService, ILogger<DriversController> logger)
    {
        _driverService = driverService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all drivers in the system.
    /// </summary>
    /// <returns>List of all drivers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DriverResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DriverResponse>>> GetAll()
    {
        _logger.LogInformation("Getting all drivers");
        var drivers = await _driverService.GetAllAsync();
        return Ok(drivers);
    }

    /// <summary>
    /// Gets a specific driver by ID.
    /// </summary>
    /// <param name="id">The driver ID</param>
    /// <returns>The driver details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponse>> GetById(Guid id)
    {
        _logger.LogInformation("Getting driver with ID: {DriverId}", id);
        var driver = await _driverService.GetByIdAsync(id);
        return Ok(driver);
    }

    /// <summary>
    /// Gets all available drivers (active and not currently on a trip).
    /// </summary>
    /// <returns>List of available drivers</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<DriverResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DriverResponse>>> GetAvailable()
    {
        _logger.LogInformation("Getting available drivers");
        var drivers = await _driverService.GetAvailableAsync();
        return Ok(drivers);
    }

    /// <summary>
    /// Creates a new driver.
    /// </summary>
    /// <param name="request">The driver creation request</param>
    /// <returns>The created driver</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DriverResponse>> Create([FromBody] CreateDriverRequest request)
    {
        _logger.LogInformation("Creating new driver with license number: {LicenseNumber}", request.LicenseNumber);
        var driver = await _driverService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
    }

    /// <summary>
    /// Updates an existing driver.
    /// </summary>
    /// <param name="id">The driver ID</param>
    /// <param name="request">The driver update request</param>
    /// <returns>The updated driver</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DriverResponse>> Update(Guid id, [FromBody] CreateDriverRequest request)
    {
        _logger.LogInformation("Updating driver with ID: {DriverId}", id);
        var driver = await _driverService.UpdateAsync(id, request);
        return Ok(driver);
    }

    /// <summary>
    /// Deletes a driver.
    /// </summary>
    /// <param name="id">The driver ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting driver with ID: {DriverId}", id);
        await _driverService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Activates a driver, allowing them to be assigned to trips.
    /// </summary>
    /// <param name="id">The driver ID</param>
    /// <returns>The updated driver</returns>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponse>> Activate(Guid id)
    {
        _logger.LogInformation("Activating driver with ID: {DriverId}", id);
        var driver = await _driverService.ActivateAsync(id);
        return Ok(driver);
    }

    /// <summary>
    /// Deactivates a driver, preventing them from being assigned to new trips.
    /// </summary>
    /// <param name="id">The driver ID</param>
    /// <returns>The updated driver</returns>
    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(typeof(DriverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponse>> Deactivate(Guid id)
    {
        _logger.LogInformation("Deactivating driver with ID: {DriverId}", id);
        var driver = await _driverService.DeactivateAsync(id);
        return Ok(driver);
    }
}
