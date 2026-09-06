using System.Collections.Concurrent;
using Asseta.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Asseta.Infrastructure.Services;

public class RedisIdempotencyService : IIdempotencyService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly ConcurrentDictionary<string, (string Value, DateTime ExpiresAtUtc)> _inMemoryStore = new();
    private readonly ConcurrentDictionary<string, DateTime> _inMemoryLocks = new();

    public RedisIdempotencyService(IConnectionMultiplexer? redis = null)
    {
        _redis = redis;
    }

    public async Task<bool> TryAcquireLockAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var lockKey = $"lock:{key}";

        if (_redis != null && _redis.IsConnected)
        {
            var db = _redis.GetDatabase();
            return await db.StringSetAsync(lockKey, "locked", expiry, When.NotExists);
        }

        // Fallback for in-memory execution
        var now = DateTime.UtcNow;
        if (_inMemoryLocks.TryGetValue(lockKey, out var currentExpiry))
        {
            if (currentExpiry > now)
            {
                return false; // Still locked
            }
        }

        _inMemoryLocks[lockKey] = now.Add(expiry);
        return true;
    }

    public async Task ReleaseLockAsync(string key, CancellationToken cancellationToken = default)
    {
        var lockKey = $"lock:{key}";

        if (_redis != null && _redis.IsConnected)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(lockKey);
            return;
        }

        _inMemoryLocks.TryRemove(lockKey, out _);
    }

    public async Task<string?> GetResponseAsync(string key, CancellationToken cancellationToken = default)
    {
        var responseKey = $"response:{key}";

        if (_redis != null && _redis.IsConnected)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(responseKey);
            return value.HasValue ? value.ToString() : null;
        }

        var now = DateTime.UtcNow;
        if (_inMemoryStore.TryGetValue(responseKey, out var entry))
        {
            if (entry.ExpiresAtUtc > now)
            {
                return entry.Value;
            }
            _inMemoryStore.TryRemove(responseKey, out _);
        }

        return null;
    }

    public async Task SaveResponseAsync(string key, string responseJson, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var responseKey = $"response:{key}";

        if (_redis != null && _redis.IsConnected)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(responseKey, responseJson, ttl);
            return;
        }

        _inMemoryStore[responseKey] = (responseJson, DateTime.UtcNow.Add(ttl));
    }
}
