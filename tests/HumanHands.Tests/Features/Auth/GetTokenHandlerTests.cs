using FluentAssertions;
using HumanHands.Features.Auth.GetToken;

namespace HumanHands.Tests.Features.Auth;

public sealed class GetTokenHandlerTests
{
    private readonly GetTokenHandler _handler = new();

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        var command = new GetTokenCommand("llm-agent-01", "secret");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsBearer()
    {
        var command = new GetTokenCommand("llm-agent-01", "secret");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Value!.TokenType.Should().Be("Bearer");
        result.Value.ExpiresIn.Should().Be(3600);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsNonEmptyToken()
    {
        var command = new GetTokenCommand("llm-agent-01", "secret");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Value!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_DifferentClientIds_ReturnsDifferentTokens()
    {
        var result1 = await _handler.Handle(new GetTokenCommand("agent-a", "x"), CancellationToken.None);
        var result2 = await _handler.Handle(new GetTokenCommand("agent-b", "x"), CancellationToken.None);

        result1.Value!.AccessToken.Should().NotBe(result2.Value!.AccessToken);
    }

    [Fact]
    public async Task Handle_ValidCredentials_TokenContainsThreeParts()
    {
        var command = new GetTokenCommand("llm-agent-01", "secret");

        var result = await _handler.Handle(command, CancellationToken.None);

        // A well-formed JWT has exactly three dot-separated segments
        result.Value!.AccessToken.Split('.').Should().HaveCount(3);
    }
}
