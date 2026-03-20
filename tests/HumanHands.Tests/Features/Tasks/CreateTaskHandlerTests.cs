using FluentAssertions;
using HumanHands.Domain.Enums;
using HumanHands.Features.Tasks.CreateTask;
using HumanHands.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;

namespace HumanHands.Tests.Features.Tasks;

public sealed class CreateTaskHandlerTests
{
    private readonly InMemoryTaskStore _store = new();
    private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        // Wire up a fake authenticated user
        var claims = new[]
        {
            new Claim("sub", "user-test-01"),
            new Claim("tenant_id", "tenant-test-01")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(principal);
        _httpContextAccessor.HttpContext.Returns(httpContext);

        _handler = new CreateTaskHandler(_store, _httpContextAccessor);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var command = new CreateTaskCommand(JobType.Errand, "Buy milk", "Corner shop");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUniqueTaskId()
    {
        var command = new CreateTaskCommand(JobType.Delivery, "Ship package", "Main St");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Value!.TaskId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ValidCommand_StatusIsPending()
    {
        var command = new CreateTaskCommand(JobType.ManualLabor, "Assemble desk", "Office");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Value!.Status.Should().Be(JobStatus.Pending);
    }

    [Fact]
    public async Task Handle_ValidCommand_TaskPersistedInStore()
    {
        var command = new CreateTaskCommand(JobType.Custom, "Fix the printer", "Room 4B");

        var result = await _handler.Handle(command, CancellationToken.None);
        var stored = _store.FindById(result.Value!.TaskId);

        stored.Should().NotBeNull();
        stored!.Description.Should().Be("Fix the printer");
        stored.Location.Should().Be("Room 4B");
        stored.JobType.Should().Be(JobType.Custom);
    }

    [Fact]
    public async Task Handle_ValidCommand_ClaimsAttachedToTask()
    {
        var command = new CreateTaskCommand(JobType.Errand, "Pick up parcel", "Post office");

        var result = await _handler.Handle(command, CancellationToken.None);
        var stored = _store.FindById(result.Value!.TaskId);

        stored!.CreatedByUserId.Should().Be("user-test-01");
        stored.TenantId.Should().Be("tenant-test-01");
    }

    [Fact]
    public async Task Handle_TwoCommands_ReturnDistinctTaskIds()
    {
        var command = new CreateTaskCommand(JobType.Errand, "Task A", "Location A");

        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        result1.Value!.TaskId.Should().NotBe(result2.Value!.TaskId);
    }
}
