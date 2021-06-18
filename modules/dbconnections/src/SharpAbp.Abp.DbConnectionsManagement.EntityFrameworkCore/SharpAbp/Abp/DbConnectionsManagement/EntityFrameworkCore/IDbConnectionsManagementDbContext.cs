using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    [ConnectionStringName(DbConnectionsManagementDbProperties.ConnectionStringName)]
    public interface IDbConnectionsManagementDbContext: IEfCoreDbContext
    {
        DbSet<DatabaseConnectionInfo> DatabaseConnectionInfos { get; set; }
    }
}
