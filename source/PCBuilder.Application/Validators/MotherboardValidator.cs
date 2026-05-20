using FluentValidation;

namespace PCBuilder.Application;

public class MotherboardValidator : AbstractValidator<MotherboardSpec>
{
    public MotherboardValidator()
    {
        RuleFor(x => x.Socket)
            .NotEmpty().WithMessage("Socket is required.");

        RuleFor(x => x.Chipset)
            .NotEmpty().WithMessage("Chipset is required.");

        RuleFor(x => x.FormFactor)
            .NotEmpty().WithMessage("Form factor is required.");

        RuleFor(x => x.MemorySlots)
            .GreaterThan(0).WithMessage("Memory slots must be greater than zero.");
    }
}
