using FluentValidation;

namespace PCBuilder.Application;

public class StorageValidator : AbstractValidator<StorageSpec>
{
    public StorageValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Storage type is required.");

        RuleFor(x => x.CapacityGb)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x.Interface)
            .NotEmpty().WithMessage("Interface is required.");

        RuleFor(x => x.ReadSpeedMb)
            .GreaterThan(0).WithMessage("Read speed must be greater than zero.");
    }
}
