using OceanBase.EntityFrameworkCore;
using SharpAbp.Abp.Data;
using SharpAbp.Abp.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class OceanBaseLargeTextColumnConventionExtensions
    {
        /// <summary>
        /// Applies the current context's OceanBase mode, including in design-time factories without ABP services.
        /// </summary>
        public static ModelConfigurationBuilder AddSharpAbpOceanBaseLargeTextColumnConvention(
            this ModelConfigurationBuilder configurationBuilder, DbContext dbContext,
            OceanBaseCompatibilityMode compatibilityMode)
        {
            AbpDbContextOptionsOceanBaseExtensions.ValidateMode(compatibilityMode);
            if (!dbContext.Database.IsOceanBase())
            {
                return configurationBuilder;
            }

            // Replace the base convention without replacing other ABP convention callbacks.
            configurationBuilder.Conventions.Remove(typeof(LargeTextColumnConvention));
            return configurationBuilder.AddSharpAbpLargeTextColumnConvention(dbContext,
                compatibilityMode == OceanBaseCompatibilityMode.MySql
                    ? OceanBaseDatabaseMode.MySql : OceanBaseDatabaseMode.Oracle);
        }
    }
}
