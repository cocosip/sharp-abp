using SharpAbp.Abp.Data;
using SharpAbp.Abp.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for ModelBuilder to work with SharpAbp database providers.
    /// </summary>
    public static class AbpModelBuilderExtensions
    {
        private const string ModelDatabaseProviderAnnotationKey = "_Abp_DatabaseProvider";

        /// <summary>
        /// Sets the SharpAbp database provider for the model.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        /// <param name="databaseProvider">The database provider to set.</param>
        public static void SetSharpAbpDatabaseProvider(
            this ModelBuilder modelBuilder,
            DatabaseProvider databaseProvider)
        {
            modelBuilder.Model.SetAnnotation(ModelDatabaseProviderAnnotationKey, databaseProvider);
        }

        /// <summary>
        /// Clears the SharpAbp database provider annotation from the model.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        public static void ClearSharpAbpDatabaseProvider(this ModelBuilder modelBuilder)
        {
            modelBuilder.Model.RemoveAnnotation(ModelDatabaseProviderAnnotationKey);
        }

        /// <summary>
        /// Gets the SharpAbp database provider from the model.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        /// <returns>The database provider if set; otherwise, null.</returns>
        public static DatabaseProvider? GetSharpAbpDatabaseProvider(this ModelBuilder modelBuilder)
        {
            var provider = modelBuilder.Model[ModelDatabaseProviderAnnotationKey];
            if (provider != null)
            {
                if (provider is Volo.Abp.EntityFrameworkCore.EfCoreDatabaseProvider efCoreDatabaseProvider)
                {
                    return efCoreDatabaseProvider.AsDatabaseProvider();
                }
                else if (provider is DatabaseProvider databaseProvider)
                {
                    return databaseProvider;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if the model is using DM (Dameng) database provider.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        /// <returns>True if using DM database provider; otherwise, false.</returns>
        public static bool IsUsingDm(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.Dm;
        }

        /// <summary>
        /// Checks if the model is using OpenGauss database provider.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        /// <returns>True if using OpenGauss database provider; otherwise, false.</returns>
        public static bool IsUsingOpenGauss(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.OpenGauss;
        }

        /// <summary>
        /// Checks if the model is using GaussDB database provider.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance.</param>
        /// <returns>True if using GaussDB database provider; otherwise, false.</returns>
        public static bool IsUsingGaussDB(this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetSharpAbpDatabaseProvider() == DatabaseProvider.GaussDB;
        }
    }
}