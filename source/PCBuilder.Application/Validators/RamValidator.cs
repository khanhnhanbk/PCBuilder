using FluentValidation;

namespace PCBuilder.Application;

public class RamValidator : AbstractValidator<RamSpec>
{
    public RamValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("RAM type is required.");

        RuleFor(x => x.CapacityGb)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x.SpeedMhz)
            .GreaterThan(0).WithMessage("Speed must be greater than zero.");

        RuleFor(x => x.Modules)
            .GreaterThan(0).WithMessage("Modules must be greater than zero.");
    }
}
