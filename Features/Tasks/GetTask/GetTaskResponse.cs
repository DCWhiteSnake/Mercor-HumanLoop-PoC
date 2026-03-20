namespace HumanHands.Features.Tasks.GetTask;

/// <summary>
/// Current snapshot of a human task. All fields are present on every response;
/// no optional or nullable fields exist to maximise LLM parseability.
/// </summary>
/// <param name="TaskId">Unique identifier of the task. Matches the TaskId from the creation response.</param>
/// <param name="Status">
/// Current lifecycle state string. Progresses linearly: Pending → InProgress → Completed.
/// Stop polling when this value equals "Completed".
/// </param>
/// <param name="StatusCode">
/// Integer representation of Status: Pending=1, InProgress=2, Completed=3.
/// Use this field for programmatic branching to avoid string comparison.
/// </param>
/// <param name="JobType">Worker category assigned to this task (e.g. "Errand", "Delivery").</param>
/// <param name="Description">Original task description supplied by the LLM at creation time.</param>
/// <param name="Location">Physical location where the task is being performed.</param>
/// <param name="CreatedAt">UTC timestamp when the task was first accepted by the API.</param>
public sealed record GetTaskResponse(
    Guid TaskId,
    string Status,
    int StatusCode,
    string JobType,
    string Description,
    string Location,
    DateTime CreatedAt
);
