using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OPAC.EntityFrameworkCore;

public class OPACHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<OPACHttpApiHostMigrationsDbContext>
{
    public OPACHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<OPACHttpApiHostMigrationsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("OPAC"));

        return new OPACHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
