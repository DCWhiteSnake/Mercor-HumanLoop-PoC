using System.Collections.Concurrent;

namespace HumanHands.Infrastructure.Persistence;

/// <summary>Represents a single intercepted API request with caller identity.</summary>
public sealed record ApiUsageEntry
{
    public DateTime Timestamp { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string TenantId { get; init; } = string.Empty;
    public string Method { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
}

/// <summary>
/// Thread-safe in-memory log of API usage entries extracted from JWT claims.
/// Registered as a singleton. Data is lost on restart.
/// </summary>
public sealed class InMemoryUsageStore
{
    private readonly ConcurrentBag<ApiUsageEntry> _log = new();

    public void Record(ApiUsageEntry entry) => _log.Add(entry);

    public IReadOnlyCollection<ApiUsageEntry> GetAll() => [.. _log];
}
