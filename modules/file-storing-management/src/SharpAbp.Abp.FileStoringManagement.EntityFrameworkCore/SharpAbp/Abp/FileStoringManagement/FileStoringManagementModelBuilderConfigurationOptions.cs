using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoringManagement
{
    public class FileStoringManagementModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public FileStoringManagementModelBuilderConfigurationOptions(
           [NotNull] string tablePrefix = "",
           [CanBeNull] string schema = null)
           : base(
               tablePrefix,
               schema)
        {

        }
    }
}


