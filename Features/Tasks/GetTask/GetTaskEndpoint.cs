using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace HumanHands.Features.Tasks.GetTask;

public static class GetTaskEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTaskQuery(id), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { Error = result.Error });
        })
        .WithName("GetTask")
        .WithTags("Tasks")
        .WithSummary("Poll the current status of a task")
        .WithDescription(
            "Returns the latest lifecycle state of a task. " +
            "Poll this endpoint until Status equals 'Completed'. " +
            "Recommended polling interval: every 15–30 seconds. " +
            "Requires a valid Bearer token obtained from POST /api/auth/token.")
        .Produces<GetTaskResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization()
        .WithMetadata(new SwaggerOperationAttribute(
            summary: "Poll the current status of a task",
            description: "Returns Status='Completed' when the human has finished the task."));
    }
}
