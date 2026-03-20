using HumanHands.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HumanHands.Features.Auth.GetToken;

public static class GetTokenEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/token", async (
            [FromBody] GetTokenCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .WithName("GetToken")
        .WithTags("Auth")
        .WithSummary("Issue a mock JWT bearer token")
        .WithDescription(
            "Authenticates a client and returns a signed JWT. " +
            "Include the returned AccessToken in the Authorization header of all task requests: " +
            "Authorization: Bearer {AccessToken}. " +
            "Any non-empty ClientId/ClientSecret pair is accepted in this mock implementation.")
        .Produces<GetTokenResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .WithMetadata(new SwaggerOperationAttribute(
            summary: "Issue a mock JWT bearer token",
            description: "Returns a bearer token required to call the protected /api/tasks endpoints."));
    }
}
