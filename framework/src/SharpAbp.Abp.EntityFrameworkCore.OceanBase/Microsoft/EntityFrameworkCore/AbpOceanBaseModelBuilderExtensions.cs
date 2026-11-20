using SharpAbp.Abp.Data;
using SharpAbp.Abp.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class AbpOceanBaseModelBuilderExtensions
    {
        public static void UseOceanBase(this ModelBuilder modelBuilder)
        {
            modelBuilder.SetSharpAbpDatabaseProvider(DatabaseProvider.OceanBase);
        }

        public static bool IsUsingOceanBase(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.OceanBase;
        }
    }
}
