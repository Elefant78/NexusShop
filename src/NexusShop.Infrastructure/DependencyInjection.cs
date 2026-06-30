using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusShop.Domain.Interfaces;
using NexusShop.Infrastructure.Persistence;
using NexusShop.Infrastructure.Persistence.Repositories;

namespace NexusShop.Infrastructure;

/// <summary>
/// Registers the EF Core context, repositories and unit of work.
/// The database provider is selected via configuration:
///   "DatabaseProvider": "Sqlite"  -> zero-setup, runs anywhere (default)
///   "DatabaseProvider": "SqlServer" -> production-grade SQL Server
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlServer"),
                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            }
            else
            {
                options.UseSqlite(
                    configuration.GetConnectionString("Sqlite") ?? "Data Source=nexusshop.db",
                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
