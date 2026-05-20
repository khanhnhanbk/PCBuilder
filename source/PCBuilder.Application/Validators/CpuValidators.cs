using FluentValidation;

namespace PCBuilder.Application;

public class CpuValidator : AbstractValidator<CpuSpec>
{
    public CpuValidator()
    {
        RuleFor(x => x.Socket)
            .NotEmpty().WithMessage("Socket is required.");

        RuleFor(x => x.Cores)
            .GreaterThan(0).WithMessage("Cores must be greater than zero.");

        RuleFor(x => x.Threads)
            .GreaterThan(0).WithMessage("Threads must be greater than zero.");

        RuleFor(x => x.BaseClockGhz)
            .GreaterThan(0).WithMessage("Base clock must be greater than zero.");

        RuleFor(x => x.BoostClockGhz)
            .GreaterThanOrEqualTo(x => x.BaseClockGhz.GetValueOrDefault())
            .WithMessage("Boost clock must be greater than or equal to base clock.");

        RuleFor(x => x.TdpWatt)
            .GreaterThan(0).WithMessage("TDP must be greater than zero.");
    }
}
