using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NexusShop.Application.Interfaces;
using NexusShop.Application.Services;

namespace NexusShop.Application;

/// <summary>
/// Registers all Application-layer services (use-cases, AutoMapper, validators)
/// with the DI container. Called from the WebAPI's Program.cs.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
