using HumanHands.Features.Auth.GetToken;

namespace HumanHands.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        GetTokenEndpoint.Map(app);
    }
}
