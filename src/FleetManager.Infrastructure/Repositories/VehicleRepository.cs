using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;
using FleetManager.Domain.Interfaces;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Vehicle entity operations using Entity Framework Core.
/// </summary>
public class VehicleRepository : IVehicleRepository
{
    private readonly FleetManagerDbContext _context;

    /// <summary>
    /// Initializes a new instance of the VehicleRepository.
    /// </summary>
    /// <param name="context">The database context</param>
    public VehicleRepository(FleetManagerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.Vehicles
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.Trips)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.Trips)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Vehicle>> GetAvailableAsync()
    {
        return await _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Available)
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.Trips)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Vehicle>> GetVehiclesWithUpcomingMaintenanceAsync(int daysThreshold)
    {
        if (daysThreshold < 0)
            throw new ArgumentException("Days threshold cannot be negative", nameof(daysThreshold));

        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

        return await _context.Vehicles
            .Where(v => v.NextMaintenanceDate <= thresholdDate)
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.Trips)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Vehicle?> GetByPlateAsync(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return null;

        return await _context.Vehicles
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.Trips)
            .FirstOrDefaultAsync(v => v.Plate == plate);
    }

    /// <inheritdoc />
    public async Task AddAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(id));

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
