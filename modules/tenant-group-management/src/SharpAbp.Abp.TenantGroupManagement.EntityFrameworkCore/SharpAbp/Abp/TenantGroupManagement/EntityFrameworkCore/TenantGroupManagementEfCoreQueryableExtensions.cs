using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    public static class TenantGroupManagementEfCoreQueryableExtensions
    {
        public static IQueryable<TenantGroup> IncludeDetails(this IQueryable<TenantGroup> queryable, bool include = true)
        {
            if (!include)
            {
                return queryable;
            }

            return queryable
                .Include(x => x.ConnectionStrings)
                .Include(x => x.Tenants);
        }
    }
}
