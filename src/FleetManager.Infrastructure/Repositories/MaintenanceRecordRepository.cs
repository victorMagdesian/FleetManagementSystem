using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for MaintenanceRecord entity operations using Entity Framework Core.
/// </summary>
public class MaintenanceRecordRepository : IMaintenanceRecordRepository
{
    private readonly FleetManagerDbContext _context;

    /// <summary>
    /// Initializes a new instance of the MaintenanceRecordRepository.
    /// </summary>
    /// <param name="context">The database context</param>
    public MaintenanceRecordRepository(FleetManagerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<MaintenanceRecord?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MaintenanceRecord>> GetByVehicleIdAsync(Guid vehicleId)
    {
        if (vehicleId == Guid.Empty)
            return Enumerable.Empty<MaintenanceRecord>();

        return await _context.MaintenanceRecords
            .Where(m => m.VehicleId == vehicleId)
            .Include(m => m.Vehicle)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(MaintenanceRecord record)
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));

        await _context.MaintenanceRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }
}
