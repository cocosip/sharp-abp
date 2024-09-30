using SharpAbp.Abp.Data;

namespace Microsoft.EntityFrameworkCore
{
    public static class AbpModelBuilderExtensions
    {
        private const string ModelDatabaseProviderAnnotationKey = "_Abp_DatabaseProvider";
        private const string ModelMultiTenancySideAnnotationKey = "_Abp_MultiTenancySide";

        public static void SetSharpAbpDatabaseProvider(
            this ModelBuilder modelBuilder,
            DatabaseProvider databaseProvider)
        {
            modelBuilder.Model.SetAnnotation(ModelDatabaseProviderAnnotationKey, databaseProvider);
        }

        public static void ClearSharpAbpDatabaseProvider(this ModelBuilder modelBuilder)
        {
            modelBuilder.Model.RemoveAnnotation(ModelDatabaseProviderAnnotationKey);
        }

        public static DatabaseProvider? GetSharpAbpDatabaseProvider(this ModelBuilder modelBuilder)
        {
            return (DatabaseProvider?)modelBuilder.Model[ModelDatabaseProviderAnnotationKey];
        }

        public static bool IsUsingDm(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.Dm;
        }
    }
}
