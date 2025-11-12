using FleetManager.Application.DTOs;
using FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FleetManager.Api.Controllers;

/// <summary>
/// Controller for managing maintenance operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(
        IMaintenanceService maintenanceService,
        IVehicleService vehicleService,
        ILogger<MaintenanceController> logger)
    {
        _maintenanceService = maintenanceService;
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Gets vehicles with upcoming maintenance within the specified threshold.
    /// </summary>
    /// <param name="daysThreshold">Number of days to look ahead (default: 7)</param>
    /// <returns>List of vehicles with upcoming maintenance</returns>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<VehicleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetUpcoming([FromQuery] int daysThreshold = 7)
    {
        _logger.LogInformation("Getting vehicles with upcoming maintenance within {DaysThreshold} days", daysThreshold);
        var vehicles = await _vehicleService.GetUpcomingMaintenanceAsync(daysThreshold);
        return Ok(vehicles);
    }

    /// <summary>
    /// Creates a new maintenance record and updates the vehicle.
    /// </summary>
    /// <param name="request">The maintenance record creation request</param>
    /// <returns>The created maintenance record</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MaintenanceRecordResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaintenanceRecordResponse>> Create([FromBody] CreateMaintenanceRecordRequest request)
    {
        _logger.LogInformation("Creating maintenance record for vehicle: {VehicleId}", request.VehicleId);
        var maintenanceRecord = await _maintenanceService.CreateAsync(request);
        return CreatedAtAction(
            nameof(GetByVehicleId),
            new { vehicleId = maintenanceRecord.VehicleId },
            maintenanceRecord);
    }

    /// <summary>
    /// Gets all maintenance records for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The vehicle ID</param>
    /// <returns>List of maintenance records</returns>
    [HttpGet("vehicle/{vehicleId}")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRecordResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MaintenanceRecordResponse>>> GetByVehicleId(Guid vehicleId)
    {
        _logger.LogInformation("Getting maintenance records for vehicle: {VehicleId}", vehicleId);
        var records = await _maintenanceService.GetByVehicleIdAsync(vehicleId);
        return Ok(records);
    }
}
