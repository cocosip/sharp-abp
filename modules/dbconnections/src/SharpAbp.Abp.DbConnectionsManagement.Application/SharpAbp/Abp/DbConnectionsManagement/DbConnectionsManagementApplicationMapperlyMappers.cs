
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FileProviderConfigurationToProviderDtoMapper : MapperBase<DatabaseConnectionInfo, DatabaseConnectionInfoDto>
    {
        public override partial DatabaseConnectionInfoDto Map(DatabaseConnectionInfo source);
        public override partial void Map(DatabaseConnectionInfo source, DatabaseConnectionInfoDto destination);
    }
}
