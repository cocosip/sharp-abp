using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SharpAbp.Abp.FileStoringManagement
{
    public static class FileStoringManagementEfCoreQueryableExtensions
    {
        public static IQueryable<FileStoringContainer> IncludeDetails(this IQueryable<FileStoringContainer> queryable, bool include = true)
        {
            if (!include)
            {
                return queryable;
            }

            return queryable
                .Include(x => x.Items);
        }
    }
}
