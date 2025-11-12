using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Driver entity operations using Entity Framework Core.
/// </summary>
public class DriverRepository : IDriverRepository
{
    private readonly FleetManagerDbContext _context;

    /// <summary>
    /// Initializes a new instance of the DriverRepository.
    /// </summary>
    /// <param name="context">The database context</param>
    public DriverRepository(FleetManagerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Driver?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.Drivers
            .Include(d => d.Trips)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Driver>> GetAllAsync()
    {
        return await _context.Drivers
            .Include(d => d.Trips)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Driver>> GetAvailableAsync()
    {
        return await _context.Drivers
            .Where(d => d.Active)
            .Include(d => d.Trips)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Driver?> GetByLicenseNumberAsync(string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            return null;

        return await _context.Drivers
            .Include(d => d.Trips)
            .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
    }

    /// <inheritdoc />
    public async Task AddAsync(Driver driver)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver));

        await _context.Drivers.AddAsync(driver);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Driver driver)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver));

        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Driver ID cannot be empty", nameof(id));

        var driver = await _context.Drivers.FindAsync(id);
        if (driver != null)
        {
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }
    }
}
