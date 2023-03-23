using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    [ConnectionStringName(TenantGroupManagementDbProperties.ConnectionStringName)]
    public interface ITenantGroupManagementDbContext : IEfCoreDbContext
    {
        DbSet<TenantGroup> TenantGroups { get; set; }
        DbSet<TenantGroupConnectionString> TenantGroupConnectionStrings { get; set; }
        DbSet<TenantGroupTenant> TenantGroupTenants { get; set; }
    }
}
