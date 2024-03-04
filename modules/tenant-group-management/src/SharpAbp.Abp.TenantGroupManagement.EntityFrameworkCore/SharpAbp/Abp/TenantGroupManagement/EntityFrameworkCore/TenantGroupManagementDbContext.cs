using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    [ConnectionStringName(TenantGroupManagementDbProperties.ConnectionStringName)]
    public class TenantGroupManagementDbContext : AbpDbContext<TenantGroupManagementDbContext>, ITenantGroupManagementDbContext
    {
        public DbSet<TenantGroup> TenantGroups { get; set; }
        public DbSet<TenantGroupConnectionString> TenantGroupConnectionStrings { get; set; }
        public DbSet<TenantGroupTenant> TenantGroupTenants { get; set; }

        public TenantGroupManagementDbContext(DbContextOptions<TenantGroupManagementDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureTenantGroupManagement();
        }
    }
}
