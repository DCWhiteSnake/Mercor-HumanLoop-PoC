using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HumanHands.Common.Models;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace HumanHands.Features.Auth.GetToken;

public sealed class GetTokenHandler : IRequestHandler<GetTokenCommand, Result<GetTokenResponse>>
{
    // Mock signing key — 256-bit minimum for HS256.
    // In production this would come from a secret vault.
    private const string SigningKey = "humanhands-mock-signing-key-32bytes!!";
    private const int ExpiresInSeconds = 3600;

    public Task<Result<GetTokenResponse>> Handle(
        GetTokenCommand request,
        CancellationToken cancellationToken)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Mock claims: derive deterministic user/tenant IDs from the ClientId.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, $"user-{request.ClientId}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("tenant_id", $"tenant-{request.ClientId}"),
            new Claim("client_id", request.ClientId)
        };

        var token = new JwtSecurityToken(
            issuer: "humanhands-api",
            audience: "humanhands-clients",
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(ExpiresInSeconds),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(Result<GetTokenResponse>.Success(
            new GetTokenResponse(tokenString, ExpiresInSeconds, "Bearer")));
    }
}
