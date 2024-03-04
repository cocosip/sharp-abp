using System.Linq;

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    public static class TransformSecurityManagementEfCoreQueryableExtensions
    {
        public static IQueryable<SecurityCredentialInfo> IncludeDetails(
            this IQueryable<SecurityCredentialInfo> queryable,
            bool include = true)
        {
            return queryable;
        }
    }
}
