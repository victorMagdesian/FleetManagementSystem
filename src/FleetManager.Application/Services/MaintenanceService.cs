using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FleetManager.Application.Services;

/// <summary>
/// Service implementation for maintenance record management operations.
/// </summary>
public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRecordRepository _maintenanceRecordRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly int _defaultMaintenanceIntervalDays;

    public MaintenanceService(
        IMaintenanceRecordRepository maintenanceRecordRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        IConfiguration configuration)
    {
        _maintenanceRecordRepository = maintenanceRecordRepository ?? throw new ArgumentNullException(nameof(maintenanceRecordRepository));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        
        // Get maintenance interval from configuration, default to 90 days if not configured
        _defaultMaintenanceIntervalDays = configuration?.GetValue<int>("Maintenance:DefaultIntervalDays") ?? 90;
    }

    /// <inheritdoc />
    public async Task<MaintenanceRecordResponse> CreateAsync(CreateMaintenanceRecordRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validate that the vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", request.VehicleId);
        }

        // Create the maintenance record
        var maintenanceRecord = _mapper.Map<MaintenanceRecord>(request);
        await _maintenanceRecordRepository.AddAsync(maintenanceRecord);

        // Update vehicle: start maintenance, complete it with the maintenance date
        vehicle.StartMaintenance();
        vehicle.CompleteMaintenance(request.Date, _defaultMaintenanceIntervalDays);

        // Persist vehicle changes
        await _vehicleRepository.UpdateAsync(vehicle);

        return _mapper.Map<MaintenanceRecordResponse>(maintenanceRecord);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MaintenanceRecordResponse>> GetByVehicleIdAsync(Guid vehicleId)
    {
        // Validate that the vehicle exists
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", vehicleId);
        }

        var maintenanceRecords = await _maintenanceRecordRepository.GetByVehicleIdAsync(vehicleId);
        return _mapper.Map<IEnumerable<MaintenanceRecordResponse>>(maintenanceRecords);
    }
}
