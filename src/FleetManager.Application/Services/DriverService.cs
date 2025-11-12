using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Application.Exceptions;
using FleetManager.Application.Interfaces;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FleetManager.Application.Services;

/// <summary>
/// Service implementation for driver management operations.
/// </summary>
public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService? _cacheService;
    private readonly ILogger<DriverService> _logger;
    private const string AvailableDriversCacheKey = "drivers:available";

    public DriverService(
        IDriverRepository driverRepository, 
        IMapper mapper, 
        ILogger<DriverService> logger,
        ICacheService? cacheService = null)
    {
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheService = cacheService;
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
        // Try to get from cache if caching is enabled
        if (_cacheService != null)
        {
            var cachedDrivers = await _cacheService.GetAsync<IEnumerable<DriverResponse>>(AvailableDriversCacheKey);
            if (cachedDrivers != null)
            {
                return cachedDrivers;
            }
        }

        // Get from repository
        var drivers = await _driverRepository.GetAvailableAsync();
        var result = _mapper.Map<IEnumerable<DriverResponse>>(drivers);

        // Cache the result if caching is enabled
        if (_cacheService != null)
        {
            await _cacheService.SetAsync(AvailableDriversCacheKey, result, 5);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<DriverResponse> CreateAsync(CreateDriverRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Creating new driver with license number: {LicenseNumber}", request.LicenseNumber);

        // Validate license number uniqueness
        var existingDriver = await _driverRepository.GetByLicenseNumberAsync(request.LicenseNumber);
        if (existingDriver != null)
        {
            _logger.LogWarning("Duplicate driver license number detected: {LicenseNumber}", request.LicenseNumber);
            throw new DuplicateEntityException("Driver", "LicenseNumber", request.LicenseNumber);
        }

        // Create new driver entity
        var driver = _mapper.Map<Driver>(request);
        
        // Add to repository
        await _driverRepository.AddAsync(driver);

        _logger.LogInformation("Driver created successfully: {DriverId}, Name: {Name}", driver.Id, driver.Name);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableDriversCacheKey);
            _logger.LogDebug("Available drivers cache invalidated");
        }

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

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableDriversCacheKey);
        }

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

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableDriversCacheKey);
        }
    }

    /// <inheritdoc />
    public async Task<DriverResponse> ActivateAsync(Guid id)
    {
        _logger.LogInformation("Activating driver: {DriverId}", id);
        
        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            _logger.LogWarning("Driver not found for activation: {DriverId}", id);
            throw new EntityNotFoundException("Driver", id);
        }

        driver.Activate();
        await _driverRepository.UpdateAsync(driver);

        _logger.LogInformation("Driver activated successfully: {DriverId}, Name: {Name}", id, driver.Name);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableDriversCacheKey);
            _logger.LogDebug("Available drivers cache invalidated");
        }

        return _mapper.Map<DriverResponse>(driver);
    }

    /// <inheritdoc />
    public async Task<DriverResponse> DeactivateAsync(Guid id)
    {
        _logger.LogInformation("Deactivating driver: {DriverId}", id);
        
        var driver = await _driverRepository.GetByIdAsync(id);
        
        if (driver == null)
        {
            _logger.LogWarning("Driver not found for deactivation: {DriverId}", id);
            throw new EntityNotFoundException("Driver", id);
        }

        driver.Deactivate();
        await _driverRepository.UpdateAsync(driver);

        _logger.LogInformation("Driver deactivated successfully: {DriverId}, Name: {Name}", id, driver.Name);

        // Invalidate cache
        if (_cacheService != null)
        {
            await _cacheService.RemoveAsync(AvailableDriversCacheKey);
            _logger.LogDebug("Available drivers cache invalidated");
        }

        return _mapper.Map<DriverResponse>(driver);
    }
}
