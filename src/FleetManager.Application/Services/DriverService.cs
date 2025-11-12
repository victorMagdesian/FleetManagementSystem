using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;

namespace FleetManager.Application.Services;

/// <summary>
/// Service implementation for driver management operations.
/// </summary>
public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public DriverService(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<DriverResponse> GetByIdAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        return _mapper.Map<DriverResponse>(driver);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DriverResponse>> GetAllAsync()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DriverResponse>>(drivers);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DriverResponse>> GetAvailableAsync()
    {
        var drivers = await _driverRepository.GetAvailableAsync();
        return _mapper.Map<IEnumerable<DriverResponse>>(drivers);
    }

    /// <inheritdoc />
    public async Task<DriverResponse> CreateAsync(CreateDriverRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validate license number uniqueness
        var existingDriver = await _driverRepository.GetByLicenseNumberAsync(request.LicenseNumber);
        if (existingDriver != null)
        {
            throw new DuplicateEntityException("Driver", "LicenseNumber", request.LicenseNumber);
        }

        // Create new driver entity
        var driver = _mapper.Map<Driver>(request);
        
        // Add to repository
        await _driverRepository.AddAsync(driver);

        return _mapper.Map<DriverResponse>(driver);
    }

    /// <inheritdoc />
    public async Task<DriverResponse> UpdateAsync(Guid id, CreateDriverRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        // Update driver information using the entity's method
        driver.UpdateInfo(request.Name, request.Phone);

        await _driverRepository.UpdateAsync(driver);

        return _mapper.Map<DriverResponse>(driver);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        await _driverRepository.DeleteAsync(id);
    }

    /// <inheritdoc />
    public async Task<DriverResponse> ActivateAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        driver.Activate();
        await _driverRepository.UpdateAsync(driver);

        return _mapper.Map<DriverResponse>(driver);
    }

    /// <inheritdoc />
    public async Task<DriverResponse> DeactivateAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        driver.Deactivate();
        await _driverRepository.UpdateAsync(driver);

        return _mapper.Map<DriverResponse>(driver);
    }
}
