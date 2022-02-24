using System.Linq;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    public static class MapTenancyManagementEfCoreQueryableExtensions
    {
        public static IQueryable<MapTenant> IncludeDetails(
            this IQueryable<MapTenant> queryable, 
            bool include = true)
        {
            return queryable;
        }
    }
}
