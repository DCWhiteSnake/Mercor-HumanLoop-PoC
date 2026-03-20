using HumanHands.Infrastructure.Persistence;

namespace HumanHands.Common.Middleware;

/// <summary>
/// Intercepts every authenticated request, extracts the user/tenant identity
/// from JWT claims, and appends a usage record to the in-memory usage log.
/// Runs after UseAuthentication() so ClaimsPrincipal is already populated.
/// Anonymous requests (e.g. POST /api/auth/token) are recorded with empty IDs.
/// </summary>
public sealed class UsageTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UsageTrackingMiddleware> _logger;

    public UsageTrackingMiddleware(RequestDelegate next, ILogger<UsageTrackingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, InMemoryUsageStore store)
    {
        var userId = context.User.FindFirst("sub")?.Value ?? string.Empty;
        var tenantId = context.User.FindFirst("tenant_id")?.Value ?? string.Empty;

        var entry = new ApiUsageEntry
        {
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            TenantId = tenantId,
            Method = context.Request.Method,
            Path = context.Request.Path
        };

        store.Record(entry);

        _logger.LogInformation(
            "API usage | user={UserId} tenant={TenantId} {Method} {Path}",
            userId, tenantId, entry.Method, entry.Path);

        await _next(context);
    }
}
