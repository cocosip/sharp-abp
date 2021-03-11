using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    [ConnectionStringName(MapTenancyManagementDbProperties.ConnectionStringName)]
    public interface IMapTenancyManagementDbContext : IEfCoreDbContext
    {
        DbSet<MapTenant> MapTenants { get; set; }
    }
}