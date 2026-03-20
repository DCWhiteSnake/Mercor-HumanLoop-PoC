using FluentAssertions;
using FluentValidation.TestHelper;
using HumanHands.Domain.Enums;
using HumanHands.Features.Tasks.CreateTask;

namespace HumanHands.Tests.Features.Tasks;

public sealed class CreateTaskValidatorTests
{
    private readonly CreateTaskValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesWithNoErrors()
    {
        var command = new CreateTaskCommand(JobType.Errand, "Buy groceries", "Local market");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyDescription_FailsValidation()
    {
        var command = new CreateTaskCommand(JobType.Errand, "", "Local market");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_EmptyLocation_FailsValidation()
    {
        var command = new CreateTaskCommand(JobType.Delivery, "Ship it", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public void Validate_DescriptionOver1000Chars_FailsValidation()
    {
        var command = new CreateTaskCommand(JobType.Custom, new string('x', 1001), "Somewhere");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_InvalidJobType_FailsValidation()
    {
        var command = new CreateTaskCommand((JobType)99, "Some task", "Somewhere");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.JobType);
    }

    [Theory]
    [InlineData(JobType.Errand)]
    [InlineData(JobType.Delivery)]
    [InlineData(JobType.ManualLabor)]
    [InlineData(JobType.Custom)]
    public void Validate_AllValidJobTypes_Pass(JobType jobType)
    {
        var command = new CreateTaskCommand(jobType, "Valid task", "Valid location");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.JobType);
    }
}
