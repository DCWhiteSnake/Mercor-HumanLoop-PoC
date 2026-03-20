using HumanHands.Domain.Enums;

namespace HumanHands.Features.Tasks.CreateTask;

/// <summary>
/// Confirmation that the task has been accepted. The LLM should store TaskId
/// and poll GET /api/tasks/{TaskId} to track progress.
/// </summary>
/// <param name="TaskId">
/// Unique identifier for this task. Use this value to poll for status updates
/// via GET /api/tasks/{TaskId}.
/// </param>
/// <param name="Status">
/// Initial lifecycle state. Always Pending on creation.
/// Poll GET /api/tasks/{TaskId} to detect transitions to InProgress and Completed.
/// </param>
/// <param name="CreatedAt">UTC timestamp when the task was accepted by the API.</param>
public sealed record CreateTaskResponse(
    Guid TaskId,
    JobStatus Status,
    DateTime CreatedAt
);
