using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Trip entity operations using Entity Framework Core.
/// </summary>
public class TripRepository : ITripRepository
{
    private readonly FleetManagerDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TripRepository.
    /// </summary>
    /// <param name="context">The database context</param>
    public TripRepository(FleetManagerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Trip?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.Trips
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Trip>> GetAllAsync()
    {
        return await _context.Trips
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Trip>> GetActiveTripsAsync()
    {
        return await _context.Trips
            .Where(t => t.EndDate == null)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Trip>> GetByVehicleIdAsync(Guid vehicleId)
    {
        if (vehicleId == Guid.Empty)
            return Enumerable.Empty<Trip>();

        return await _context.Trips
            .Where(t => t.VehicleId == vehicleId)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Trip>> GetByDriverIdAsync(Guid driverId)
    {
        if (driverId == Guid.Empty)
            return Enumerable.Empty<Trip>();

        return await _context.Trips
            .Where(t => t.DriverId == driverId)
            .Include(t => t.Vehicle)
            .Include(t => t.Driver)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddAsync(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullException(nameof(trip));

        await _context.Trips.AddAsync(trip);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullException(nameof(trip));

        _context.Trips.Update(trip);
        await _context.SaveChangesAsync();
    }
}
