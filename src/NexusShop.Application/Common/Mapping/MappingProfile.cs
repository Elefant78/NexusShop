using AutoMapper;
using NexusShop.Domain.Entities;
using NexusShop.Application.DTOs;

namespace NexusShop.Application.Common.Mapping;

/// <summary>
/// Central AutoMapper profile that maps Domain entities to DTOs.
/// The Money value object is flattened into Price + Currency.
/// </summary>
public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null));

        CreateMap<Category, CategoryDto>();
    }
}
