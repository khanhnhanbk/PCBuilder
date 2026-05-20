using FluentValidation;

namespace PCBuilder.Application;

public class PsuValidator : AbstractValidator<PsuSpec>
{
    public PsuValidator()
    {
        RuleFor(x => x.Wattage)
            .GreaterThan(0).WithMessage("Wattage must be greater than zero.");

        RuleFor(x => x.Efficiency)
            .NotEmpty().WithMessage("Efficiency rating is required.");

        RuleFor(x => x.ModularType)
            .NotEmpty().WithMessage("Modular type is required.");
    }
}
