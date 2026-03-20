using HumanHands.Domain.Enums;

namespace HumanHands.Domain.Entities;

/// <summary>Represents a unit of work delegated by an LLM to a human agent.</summary>
public sealed class TaskItem
{
    public Guid Id { get; init; }
    public JobType JobType { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public JobStatus Status { get; set; }
    public DateTime CreatedAt { get; init; }
    public string CreatedByUserId { get; init; } = string.Empty;
    public string TenantId { get; init; } = string.Empty;
}
