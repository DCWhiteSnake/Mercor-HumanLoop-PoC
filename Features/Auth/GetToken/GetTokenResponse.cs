namespace HumanHands.Features.Auth.GetToken;

/// <summary>
/// A short-lived bearer token the LLM must include in the Authorization header
/// of every subsequent request: <c>Authorization: Bearer {AccessToken}</c>.
/// </summary>
/// <param name="AccessToken">Signed JWT bearer token. Valid for 1 hour.</param>
/// <param name="ExpiresIn">Token lifetime in seconds (3600 = 1 hour).</param>
/// <param name="TokenType">Always "Bearer". Pass this value as the Authorization scheme.</param>
public sealed record GetTokenResponse(
    string AccessToken,
    int ExpiresIn,
    string TokenType
);
