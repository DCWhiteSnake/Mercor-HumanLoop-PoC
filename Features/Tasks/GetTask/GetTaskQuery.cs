using HumanHands.Common.Models;
using MediatR;

namespace HumanHands.Features.Tasks.GetTask;

/// <summary>
/// Polls the current status of a previously created task.
/// Call repeatedly until Status equals "Completed".
/// </summary>
/// <param name="Id">
/// The unique task identifier returned by POST /api/tasks.
/// Must be a valid UUID (GUID format).
/// </param>
public sealed record GetTaskQuery(Guid Id) : IRequest<Result<GetTaskResponse>>;
