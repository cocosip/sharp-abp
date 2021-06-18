using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    [ConnectionStringName(DbConnectionsManagementDbProperties.ConnectionStringName)]
    public class DbConnectionsManagementDbContext : AbpDbContext<DbConnectionsManagementDbContext>, IDbConnectionsManagementDbContext
    {
        public DbSet<DatabaseConnectionInfo> DatabaseConnectionInfos { get; set; }

        public DbConnectionsManagementDbContext(DbContextOptions<DbConnectionsManagementDbContext> options)
            : base(options)
        {
        }
    }
}
