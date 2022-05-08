using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MinIdApp.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class MinIdAppMigrationsDbContextFactory : IDesignTimeDbContextFactory<MinIdAppMigrationsDbContext>
    {
        public MinIdAppMigrationsDbContext CreateDbContext(string[] args)
        {
            MinIdAppEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<MinIdAppMigrationsDbContext>()
                .UseNpgsql(configuration.GetConnectionString("Default"));

            return new MinIdAppMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MinIdApp.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
