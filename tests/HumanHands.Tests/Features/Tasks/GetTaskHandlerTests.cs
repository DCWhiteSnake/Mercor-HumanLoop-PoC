using FluentAssertions;
using HumanHands.Domain.Entities;
using HumanHands.Domain.Enums;
using HumanHands.Features.Tasks.GetTask;
using HumanHands.Infrastructure.Persistence;

namespace HumanHands.Tests.Features.Tasks;

public sealed class GetTaskHandlerTests
{
    private readonly InMemoryTaskStore _store = new();
    private readonly GetTaskHandler _handler;

    public GetTaskHandlerTests()
    {
        _handler = new GetTaskHandler(_store);
    }

    private TaskItem SeedTask(DateTime? createdAt = null)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            JobType = JobType.Delivery,
            Description = "Deliver flowers",
            Location = "42 Rose Ave",
            Status = JobStatus.Pending,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            CreatedByUserId = "user-001",
            TenantId = "tenant-001"
        };
        _store.Add(task);
        return task;
    }

    [Fact]
    public async Task Handle_ExistingTask_ReturnsSuccess()
    {
        var task = SeedTask();

        var result = await _handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UnknownId_ReturnsFailure()
    {
        var result = await _handler.Handle(new GetTaskQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_FreshTask_StatusIsPending()
    {
        var task = SeedTask(DateTime.UtcNow);

        var result = await _handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);

        result.Value!.Status.Should().Be("Pending");
        result.Value.StatusCode.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Task31SecondsOld_StatusIsInProgress()
    {
        var task = SeedTask(DateTime.UtcNow.AddSeconds(-31));

        var result = await _handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);

        result.Value!.Status.Should().Be("InProgress");
        result.Value.StatusCode.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Task121SecondsOld_StatusIsCompleted()
    {
        var task = SeedTask(DateTime.UtcNow.AddSeconds(-121));

        var result = await _handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);

        result.Value!.Status.Should().Be("Completed");
        result.Value.StatusCode.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ExistingTask_ResponseFieldsMatch()
    {
        var task = SeedTask();

        var result = await _handler.Handle(new GetTaskQuery(task.Id), CancellationToken.None);
        var response = result.Value!;

        response.TaskId.Should().Be(task.Id);
        response.JobType.Should().Be("Delivery");
        response.Description.Should().Be("Deliver flowers");
        response.Location.Should().Be("42 Rose Ave");
    }
}
