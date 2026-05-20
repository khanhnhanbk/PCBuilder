using FluentValidation;

namespace PCBuilder.Application;

public class GpuValidator : AbstractValidator<GpuSpec>
{
    public GpuValidator()
    {
        RuleFor(x => x.VramGb)
            .GreaterThan(0).WithMessage("VRAM must be greater than zero.");

        RuleFor(x => x.MemoryType)
            .NotEmpty().WithMessage("Memory type is required.");

        RuleFor(x => x.TdpWatt)
            .GreaterThan(0).WithMessage("TDP must be greater than zero.");

        RuleFor(x => x.LengthMm)
            .GreaterThan(0).WithMessage("Length must be greater than zero.");
    }
}
