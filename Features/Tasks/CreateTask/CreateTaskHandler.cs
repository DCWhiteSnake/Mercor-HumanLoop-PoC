using HumanHands.Common.Models;
using HumanHands.Domain.Entities;
using HumanHands.Domain.Enums;
using HumanHands.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HumanHands.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<CreateTaskResponse>>
{
    private readonly InMemoryTaskStore _store;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateTaskHandler(InMemoryTaskStore store, IHttpContextAccessor httpContextAccessor)
    {
        _store = store;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Result<CreateTaskResponse>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst("sub")?.Value ?? string.Empty;
        var tenantId = user?.FindFirst("tenant_id")?.Value ?? string.Empty;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            JobType = request.JobType,
            Description = request.Description,
            Location = request.Location,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId,
            TenantId = tenantId
        };

        _store.Add(task);

        return Task.FromResult(Result<CreateTaskResponse>.Success(
            new CreateTaskResponse(task.Id, task.Status, task.CreatedAt)));
    }
}
