using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FleetManager.Infrastructure.Jobs;

/// <summary>
/// Background job that checks for vehicles with upcoming maintenance and logs alerts.
/// Executes daily at midnight to identify vehicles requiring maintenance within the threshold.
/// </summary>
public class MaintenanceCheckJob : IJob
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<MaintenanceCheckJob> _logger;
    private const int DaysThreshold = 3;

    /// <summary>
    /// Initializes a new instance of the MaintenanceCheckJob.
    /// </summary>
    /// <param name="vehicleRepository">Repository for vehicle data access</param>
    /// <param name="logger">Logger for recording maintenance alerts</param>
    public MaintenanceCheckJob(
        IVehicleRepository vehicleRepository,
        ILogger<MaintenanceCheckJob> logger)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the maintenance check job.
    /// Queries vehicles with upcoming maintenance and logs alert messages.
    /// </summary>
    /// <param name="context">Job execution context provided by Quartz</param>
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("MaintenanceCheckJob started at {ExecutionTime}", DateTime.UtcNow);

        try
        {
            var vehicles = await _vehicleRepository.GetVehiclesWithUpcomingMaintenanceAsync(DaysThreshold);
            var vehicleList = vehicles.ToList();

            foreach (var vehicle in vehicleList)
            {
                _logger.LogInformation(
                    "[ALERTA] Veículo {Plate} com manutenção próxima ({NextMaintenanceDate:dd/MM/yyyy})",
                    vehicle.Plate,
                    vehicle.NextMaintenanceDate);
            }

            _logger.LogInformation(
                "MaintenanceCheckJob executado. {Count} veículos com manutenção próxima.",
                vehicleList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing MaintenanceCheckJob");
            throw;
        }
    }
}
