namespace HumanHands.Domain.Enums;

/// <summary>
/// Lifecycle state of a human task. Progresses linearly: Pending → InProgress → Completed.
/// An LLM should poll GET /api/tasks/{id} until the status reaches Completed.
/// </summary>
public enum JobStatus
{
    /// <summary>Task has been accepted and is queued; no human has been assigned yet.</summary>
    Pending = 1,

    /// <summary>A human agent has been assigned and is actively working on the task.</summary>
    InProgress = 2,

    /// <summary>The task has been finished successfully. No further polling is required.</summary>
    Completed = 3
}
