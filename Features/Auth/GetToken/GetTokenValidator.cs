using FluentValidation;

namespace HumanHands.Features.Auth.GetToken;

public sealed class GetTokenValidator : AbstractValidator<GetTokenCommand>
{
    public GetTokenValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.ClientSecret).NotEmpty();
    }
}
