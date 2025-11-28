using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MapTenantToMapTenantEtoMapper : MapperBase<MapTenant, MapTenantEto>
    {
        [MapperIgnoreTarget(nameof(MapTenantEto.KeysAsString))]
        [MapperIgnoreTarget(nameof(MapTenantEto.EntityType))]
        public override partial MapTenantEto Map(MapTenant source);

        [MapperIgnoreTarget(nameof(MapTenantEto.KeysAsString))]
        [MapperIgnoreTarget(nameof(MapTenantEto.EntityType))]
        public override partial void Map(MapTenant source, MapTenantEto destination);
    }
}
