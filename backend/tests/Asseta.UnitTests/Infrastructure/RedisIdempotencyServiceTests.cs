using Asseta.Infrastructure.Services;
using Xunit;

namespace Asseta.UnitTests.Infrastructure;

public class RedisIdempotencyServiceTests
{
    [Fact]
    public async Task TryAcquireLockAsync_FirstAttempt_ShouldSucceed()
    {
        var service = new RedisIdempotencyService();
        var key = Guid.NewGuid().ToString();

        var acquired = await service.TryAcquireLockAsync(key, TimeSpan.FromSeconds(5));

        Assert.True(acquired);
    }

    [Fact]
    public async Task TryAcquireLockAsync_ConcurrentAttempt_ShouldFail()
    {
        var service = new RedisIdempotencyService();
        var key = Guid.NewGuid().ToString();

        var first = await service.TryAcquireLockAsync(key, TimeSpan.FromSeconds(5));
        var second = await service.TryAcquireLockAsync(key, TimeSpan.FromSeconds(5));

        Assert.True(first);
        Assert.False(second);
    }

    [Fact]
    public async Task ReleaseLockAsync_ShouldAllowAcquiringAgain()
    {
        var service = new RedisIdempotencyService();
        var key = Guid.NewGuid().ToString();

        await service.TryAcquireLockAsync(key, TimeSpan.FromSeconds(5));
        await service.ReleaseLockAsync(key);

        var reacquired = await service.TryAcquireLockAsync(key, TimeSpan.FromSeconds(5));
        Assert.True(reacquired);
    }

    [Fact]
    public async Task SaveResponseAsync_And_GetResponseAsync_ShouldReturnSavedPayload()
    {
        var service = new RedisIdempotencyService();
        var key = Guid.NewGuid().ToString();
        var payload = "{\"success\": true, \"data\": {\"id\": \"123\"}}";

        await service.SaveResponseAsync(key, payload, TimeSpan.FromMinutes(10));
        var retrieved = await service.GetResponseAsync(key);

        Assert.Equal(payload, retrieved);
    }

    [Fact]
    public async Task GetResponseAsync_NonExistentKey_ShouldReturnNull()
    {
        var service = new RedisIdempotencyService();
        var key = Guid.NewGuid().ToString();

        var retrieved = await service.GetResponseAsync(key);

        Assert.Null(retrieved);
    }
}
