using FluentValidation;
using HumanHands.Domain.Enums;

namespace HumanHands.Features.Tasks.CreateTask;

public sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.JobType)
            .IsInEnum()
            .WithMessage("JobType must be one of: Errand (1), Delivery (2), ManualLabor (3), Custom (4).");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Description is required and must not exceed 1000 characters.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Location is required and must not exceed 500 characters.");
    }
}
