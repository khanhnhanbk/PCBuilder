using System.Text.Json;
using FluentValidation;
using PCBuilder.Domain;

namespace PCBuilder.Application;

public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters.");

        RuleFor(x => x.Model)
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).When(x => x.Price.HasValue)
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.SpecsJson)
            .Must(x => x.ValueKind == JsonValueKind.Object)
            .WithMessage("SpecsJson must be a JSON object.");

        RuleFor(x => x)
            .Custom(ValidateSpecs);
    }

    private void ValidateSpecs(ProductUpdateDto dto, dynamic context)
    {
        if (dto.SpecsJson.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        switch (dto.Type)
        {
            case ProductTypeEnum.CPU:
                ValidateJsonSpec(dto, context, new CpuValidator());
                break;
            case ProductTypeEnum.MOTHERBOARD:
                ValidateJsonSpec(dto, context, new MotherboardValidator());
                break;
            case ProductTypeEnum.RAM:
                ValidateJsonSpec(dto, context, new RamValidator());
                break;
            case ProductTypeEnum.GPU:
                ValidateJsonSpec(dto, context, new GpuValidator());
                break;
            case ProductTypeEnum.PSU:
                ValidateJsonSpec(dto, context, new PsuValidator());
                break;
            case ProductTypeEnum.CASE:
                ValidateJsonSpec(dto, context, new CaseValidator());
                break;
            case ProductTypeEnum.STORAGE:
                ValidateJsonSpec(dto, context, new StorageValidator());
                break;
            default:
                context.AddFailure("Type", "Unsupported product type.");
                break;
        }
    }

    private void ValidateJsonSpec<T>(ProductUpdateDto dto, dynamic context, IValidator<T> validator)
        where T : class
    {
        try
        {
            var spec = JsonSerializer.Deserialize<T>(dto.SpecsJson.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (spec is null)
            {
                context.AddFailure("SpecsJson", "SpecsJson must contain a valid specification object.");
                return;
            }

            var result = validator.Validate(spec);
            foreach (var error in result.Errors)
            {
                context.AddFailure($"SpecsJson.{error.PropertyName}", error.ErrorMessage);
            }
        }
        catch (JsonException)
        {
            context.AddFailure("SpecsJson", "SpecsJson must contain valid JSON.");
        }
    }
}
