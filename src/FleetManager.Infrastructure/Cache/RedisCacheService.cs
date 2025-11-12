using FleetManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace FleetManager.Infrastructure.Cache;

/// <summary>
/// Redis implementation of the cache service.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly int _defaultExpirationMinutes;

    public RedisCacheService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _database = _redis.GetDatabase();
        _defaultExpirationMinutes = configuration.GetValue<int>("CacheSettings:DefaultExpirationMinutes", 10);
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
        }

        var value = await _database.StringGetAsync(key);
        
        if (!value.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(value!);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, int? expirationMinutes = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var serializedValue = JsonSerializer.Serialize(value);
        var expiration = TimeSpan.FromMinutes(expirationMinutes ?? _defaultExpirationMinutes);

        await _database.StringSetAsync(key, serializedValue, expiration);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
        }

        await _database.KeyDeleteAsync(key);
    }

    /// <inheritdoc />
    public async Task RemoveByPatternAsync(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));
        }

        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());

        var keys = server.Keys(pattern: pattern).ToArray();

        if (keys.Length > 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
