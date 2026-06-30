using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NexusShop.Infrastructure.Persistence;

/// <summary>
/// Allows the EF Core CLI tools (dotnet ef migrations add ...) to create the
/// context at design time without running the WebAPI host. Uses SQLite so the
/// command works on any machine.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=nexusshop.design.db")
            .Options;

        return new ApplicationDbContext(options);
    }
}
