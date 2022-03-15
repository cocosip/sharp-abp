using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AllApiSample.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class AllApiSampleDbContextFactory : IDesignTimeDbContextFactory<AllApiSampleDbContext>
{
    public AllApiSampleDbContext CreateDbContext(string[] args)
    {
        AllApiSampleEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();
        //
        var builder = new DbContextOptionsBuilder<AllApiSampleDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new AllApiSampleDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AllApiSample.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
