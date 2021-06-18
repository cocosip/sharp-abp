using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SharpAbp.Abp.DbConnectionsManagement.EntityFrameworkCore
{
    public class DbConnectionsManagementModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public DbConnectionsManagementModelBuilderConfigurationOptions(
           [NotNull] string tablePrefix = "",
           [CanBeNull] string schema = null)
           : base(
               tablePrefix,
               schema)
        {

        }
    }
}
