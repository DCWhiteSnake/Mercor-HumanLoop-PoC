using HumanHands.Common.Models;
using HumanHands.Domain.Enums;
using HumanHands.Infrastructure.Persistence;
using MediatR;

namespace HumanHands.Features.Tasks.GetTask;

public sealed class GetTaskHandler : IRequestHandler<GetTaskQuery, Result<GetTaskResponse>>
{
    private readonly InMemoryTaskStore _store;

    public GetTaskHandler(InMemoryTaskStore store) => _store = store;

    public Task<Result<GetTaskResponse>> Handle(
        GetTaskQuery request,
        CancellationToken cancellationToken)
    {
        var task = _store.FindById(request.Id);

        if (task is null)
            return Task.FromResult(Result<GetTaskResponse>.Failure($"Task '{request.Id}' not found."));

        // Simulate deterministic status progression based on elapsed time.
        // < 30 s  → Pending
        // 30–120 s → InProgress
        // > 120 s  → Completed
        var age = DateTime.UtcNow - task.CreatedAt;
        var simulatedStatus = age.TotalSeconds switch
        {
            < 30 => JobStatus.Pending,
            < 120 => JobStatus.InProgress,
            _ => JobStatus.Completed
        };

        task.Status = simulatedStatus;

        var response = new GetTaskResponse(
            TaskId: task.Id,
            Status: simulatedStatus.ToString(),
            StatusCode: (int)simulatedStatus,
            JobType: task.JobType.ToString(),
            Description: task.Description,
            Location: task.Location,
            CreatedAt: task.CreatedAt);

        return Task.FromResult(Result<GetTaskResponse>.Success(response));
    }
}
