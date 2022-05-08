using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.MinId.EntityFrameworkCore
{
    public class MinIdModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public MinIdModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}