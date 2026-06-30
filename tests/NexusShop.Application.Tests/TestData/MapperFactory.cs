using AutoMapper;
using NexusShop.Application.Common.Mapping;

namespace NexusShop.Application.Tests.TestData;

/// <summary>Builds a real AutoMapper instance from the production MappingProfile.</summary>
public static class MapperFactory
{
    public static IMapper Create()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        configuration.AssertConfigurationIsValid();
        return configuration.CreateMapper();
    }
}
