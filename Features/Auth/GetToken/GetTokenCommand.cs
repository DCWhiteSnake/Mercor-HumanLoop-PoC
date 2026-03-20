using MediatR;
using HumanHands.Common.Models;

namespace HumanHands.Features.Auth.GetToken;

/// <summary>
/// Request to obtain a mock JWT bearer token.
/// Provide any non-empty ClientId and ClientSecret to receive a token.
/// </summary>
/// <param name="ClientId">Identifier of the calling LLM agent or application.</param>
/// <param name="ClientSecret">Shared secret for the client (any non-empty string is accepted in this mock).</param>
public sealed record GetTokenCommand(
    string ClientId,
    string ClientSecret
) : IRequest<Result<GetTokenResponse>>;
