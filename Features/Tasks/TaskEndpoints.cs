using HumanHands.Features.Tasks.CreateTask;
using HumanHands.Features.Tasks.GetTask;

namespace HumanHands.Features.Tasks;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        CreateTaskEndpoint.Map(app);
        GetTaskEndpoint.Map(app);
    }
}
