using FluentValidation;

namespace PCBuilder.Application;

public class CaseValidator : AbstractValidator<CaseSpec>
{
    public CaseValidator()
    {
        RuleFor(x => x.FormFactor)
            .NotEmpty().WithMessage("Form factor is required.");

        RuleFor(x => x.FanSlots)
            .GreaterThanOrEqualTo(0).WithMessage("Fan slots must be zero or greater.");

        RuleFor(x => x.MaxGpuLengthMm)
            .GreaterThan(0).WithMessage("Max GPU length must be greater than zero.");
    }
}
