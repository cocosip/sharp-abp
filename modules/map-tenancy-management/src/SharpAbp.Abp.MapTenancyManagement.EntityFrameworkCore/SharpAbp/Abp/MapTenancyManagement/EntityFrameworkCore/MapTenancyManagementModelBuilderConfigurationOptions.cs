using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.MapTenancyManagement.EntityFrameworkCore
{
    public class MapTenancyManagementModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public MapTenancyManagementModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}