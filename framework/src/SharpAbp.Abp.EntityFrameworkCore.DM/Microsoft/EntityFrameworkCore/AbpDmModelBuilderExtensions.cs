using SharpAbp.Abp.Data;

namespace Microsoft.EntityFrameworkCore
{
    public static class AbpDmModelBuilderExtensions
    {
        public static void UseDm(this ModelBuilder modelBuilder)
        {
            modelBuilder.SetSharpAbpDatabaseProvider(DatabaseProvider.Dm);
        }

        public static bool IsUsingDm(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.Dm;
        }
    }
}
