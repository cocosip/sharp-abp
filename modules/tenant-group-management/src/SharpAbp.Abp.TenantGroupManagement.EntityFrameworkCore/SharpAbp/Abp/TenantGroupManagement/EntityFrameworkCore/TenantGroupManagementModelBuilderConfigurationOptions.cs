using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.TenantGroupManagement.EntityFrameworkCore
{
    public class TenantGroupManagementModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public TenantGroupManagementModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(tablePrefix, schema)
        {

        }
    }
}
