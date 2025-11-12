namespace FleetManager.Application.Interfaces;

/// <summary>
/// Service interface for caching operations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value</typeparam>
    /// <param name="key">The cache key</param>
    /// <returns>The cached value, or default if not found</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Sets a value in the cache with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expirationMinutes">Optional expiration time in minutes</param>
    Task SetAsync<T>(string key, T value, int? expirationMinutes = null) where T : class;

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    /// <param name="key">The cache key</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes all cached values matching a pattern.
    /// </summary>
    /// <param name="pattern">The key pattern (e.g., "vehicles:*")</param>
    Task RemoveByPatternAsync(string pattern);
}
