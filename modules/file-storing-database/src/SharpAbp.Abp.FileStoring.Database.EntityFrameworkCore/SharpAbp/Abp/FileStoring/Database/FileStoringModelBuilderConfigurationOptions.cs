using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoring.Database
{
    public class FileStoringModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public FileStoringModelBuilderConfigurationOptions(
           [NotNull] string tablePrefix = "",
           [CanBeNull] string schema = null)
           : base(
               tablePrefix,
               schema)
        {

        }
    }
}
