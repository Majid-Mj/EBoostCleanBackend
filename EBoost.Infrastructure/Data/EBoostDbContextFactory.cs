using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EBoost.Infrastructure.Data;

public class EBoostDbContextFactory
    : IDesignTimeDbContextFactory<EBoostDbContext>
{
    public EBoostDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<EBoostDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Graceful design-time fallback for CLI tools if running against a sanitized appsettings file
        if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_AZURE_SQL_CONNECTION_STRING"))
        {
            connectionString = "Server=.;Database=EBoost_EcommerceDb;Trusted_Connection=True;TrustServerCertificate=True";
        }

        optionsBuilder.UseSqlServer(connectionString);

        return new EBoostDbContext(optionsBuilder.Options);
    }
}