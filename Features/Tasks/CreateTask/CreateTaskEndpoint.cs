using HumanHands.Features.Tasks.CreateTask;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HumanHands.Features.Tasks.CreateTask;

public static class CreateTaskEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks", async (
            [FromBody] CreateTaskCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.AcceptedAtRoute(
                    "GetTask",
                    new { id = result.Value!.TaskId },
                    result.Value)
                : Results.BadRequest(result.Error);
        })
        .WithName("CreateTask")
        .WithTags("Tasks")
        .WithSummary("Delegate a physical task to a human agent")
        .WithDescription(
            "Accepts a task definition and enqueues it for human execution. " +
            "Returns 202 Accepted immediately; the task is not yet started. " +
            "Store the returned TaskId and poll GET /api/tasks/{TaskId} to track lifecycle. " +
            "Requires a valid Bearer token obtained from POST /api/auth/token.")
        .Produces<CreateTaskResponse>(StatusCodes.Status202Accepted)
        .ProducesValidationProblem()
        .RequireAuthorization()
        .WithMetadata(new SwaggerOperationAttribute(
            summary: "Delegate a physical task to a human agent",
            description: "Enqueues a new task. Poll GET /api/tasks/{id} for status updates."));
    }
}
