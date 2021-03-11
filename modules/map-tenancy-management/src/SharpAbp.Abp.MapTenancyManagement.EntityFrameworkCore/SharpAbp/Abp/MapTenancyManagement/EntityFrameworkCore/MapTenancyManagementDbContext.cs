using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    [ConnectionStringName(MapTenancyManagementDbProperties.ConnectionStringName)]
    public class MapTenancyManagementDbContext : AbpDbContext<MapTenancyManagementDbContext>, IMapTenancyManagementDbContext
    {
        public DbSet<MapTenant> MapTenants { get; set; }

        public MapTenancyManagementDbContext(DbContextOptions<MapTenancyManagementDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureMapTenancyManagement();
        }
    }
}