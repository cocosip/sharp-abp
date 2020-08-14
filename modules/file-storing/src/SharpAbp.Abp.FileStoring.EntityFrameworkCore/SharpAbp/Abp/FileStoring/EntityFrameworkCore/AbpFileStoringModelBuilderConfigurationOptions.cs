using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoring.EntityFrameworkCore
{
    public class AbpFileStoringModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public AbpFileStoringModelBuilderConfigurationOptions(
           [NotNull] string tablePrefix,
           [CanBeNull] string schema)
           : base(
               tablePrefix,
               schema)
        {

        }
    }
}
