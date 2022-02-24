using System.Linq;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    public static class DbConnectionsManagementEfCoreQueryableExtensions
    {
        public static IQueryable<DatabaseConnectionInfo> IncludeDetails(
            this IQueryable<DatabaseConnectionInfo> queryable,
            bool include = true)
        {
            return queryable;
        }
    }
}
