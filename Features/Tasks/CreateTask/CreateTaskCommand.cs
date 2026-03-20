using HumanHands.Common.Models;
using HumanHands.Domain.Enums;
using MediatR;

namespace HumanHands.Features.Tasks.CreateTask;

/// <summary>
/// Payload for delegating a physical task to a human agent.
/// All three fields are required. The LLM must populate Description with
/// enough context for the human worker to act without further clarification.
/// </summary>
/// <param name="JobType">
/// Category of work. Determines which human worker pool receives the task.
/// Valid values: Errand (1), Delivery (2), ManualLabor (3), Custom (4).
/// </param>
/// <param name="Description">
/// Natural-language description of what the human must do.
/// Be specific: include item names, quantities, contact names, and special instructions.
/// Maximum 1000 characters.
/// </param>
/// <param name="Location">
/// Physical address or place name where the task must be performed or collected from.
/// Example: "123 Main St, Springfield" or "Central Park, NY".
/// </param>
public sealed record CreateTaskCommand(
    JobType JobType,
    string Description,
    string Location
) : IRequest<Result<CreateTaskResponse>>;
