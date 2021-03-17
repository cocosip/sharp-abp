using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SharpAbp.WebSample.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class WebSampleMigrationsDbContextFactory : IDesignTimeDbContextFactory<WebSampleMigrationsDbContext>
    {
        public WebSampleMigrationsDbContext CreateDbContext(string[] args)
        {
            WebSampleEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<WebSampleMigrationsDbContext>()
                .UseSqlite(configuration.GetConnectionString("Default"));

            return new WebSampleMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SharpAbp.WebSample.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
