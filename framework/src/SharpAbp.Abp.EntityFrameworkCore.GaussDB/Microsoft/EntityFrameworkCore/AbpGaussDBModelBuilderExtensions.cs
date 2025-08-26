using SharpAbp.Abp.Data;

namespace Microsoft.EntityFrameworkCore
{
    public static class AbpGaussDBModelBuilderExtensions
    {
        public static void UseGaussDB(this ModelBuilder modelBuilder)
        {
            modelBuilder.SetSharpAbpDatabaseProvider(DatabaseProvider.GaussDB);
        }

        public static bool IsUsingGaussDB(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.GaussDB;
        }
    }
}
