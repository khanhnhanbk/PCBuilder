using AutoMapper;
using PCBuilder.Application;
using PCBuilder.Domain;
using System.Text.Json;

namespace PCBuilder.API.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.SpecsJson, opt => opt.MapFrom(src => src.SpecsJson.GetRawText()))
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ProductUpdateDto, Product>()
            .ForMember(dest => dest.SpecsJson, opt => opt.MapFrom(src => src.SpecsJson.GetRawText()))
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Product, ProductReadDto>()
            .ForMember(dest => dest.Specs, opt => opt.MapFrom((src, dest) => DeserializeSpecs(src.SpecsJson)));
    }

    private static object? DeserializeSpecs(string specsJson)
    {
        if (!string.IsNullOrWhiteSpace(specsJson) && specsJson != "{}")
        {
            return JsonSerializer.Deserialize<object>(specsJson);
        }
        return null;
    }
}

