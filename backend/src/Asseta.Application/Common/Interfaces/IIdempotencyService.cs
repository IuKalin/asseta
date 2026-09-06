namespace Asseta.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<bool> TryAcquireLockAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task ReleaseLockAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetResponseAsync(string key, CancellationToken cancellationToken = default);
    Task SaveResponseAsync(string key, string responseJson, TimeSpan ttl, CancellationToken cancellationToken = default);
}
