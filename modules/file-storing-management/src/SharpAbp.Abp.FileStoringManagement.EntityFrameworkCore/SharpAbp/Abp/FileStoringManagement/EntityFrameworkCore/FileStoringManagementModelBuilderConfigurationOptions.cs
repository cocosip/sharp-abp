using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore
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


