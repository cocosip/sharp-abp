using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.TransformSecurityManagement.EntityFrameworkCore
{
    public class AbpTransformSecurityManagementModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public AbpTransformSecurityManagementModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null) : base(tablePrefix, schema)
        {

        }
    }
}
