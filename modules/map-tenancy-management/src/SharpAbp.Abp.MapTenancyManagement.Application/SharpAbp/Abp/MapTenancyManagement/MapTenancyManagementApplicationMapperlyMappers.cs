using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MapTenantToMapTenantDtoMapper : MapperBase<MapTenant, MapTenantDto>
    {
        public override partial MapTenantDto Map(MapTenant source);
        public override partial void Map(MapTenant source, MapTenantDto destination);
    }

    // [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    // public partial class CreateMapTenantDtoToMapTenantMapper : MapperBase<CreateMapTenantDto, MapTenant>
    // {
    //     public override partial MapTenant Map(CreateMapTenantDto source);
    //     public override partial void Map(CreateMapTenantDto source, MapTenant destination);
    // }

    // [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    // public partial class UpdateMapTenantDtoToMapTenantMapper : MapperBase<UpdateMapTenantDto, MapTenant>
    // {
    //     public override partial MapTenant Map(UpdateMapTenantDto source);
    //     public override partial void Map(UpdateMapTenantDto source, MapTenant destination);
    // }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class TenantToHybridMapTenantDtoMapper : MapperBase<Tenant, HybridMapTenantDto>
    {
        [MapProperty(nameof(Tenant.Id), nameof(HybridMapTenantDto.TenantId))]
        [MapperIgnoreTarget(nameof(HybridMapTenantDto.TenantName))]
        [MapperIgnoreTarget(nameof(HybridMapTenantDto.Code))]
        [MapperIgnoreTarget(nameof(HybridMapTenantDto.MapCode))]
        public override partial HybridMapTenantDto Map(Tenant source);
        [MapProperty(nameof(Tenant.Id), nameof(HybridMapTenantDto.TenantId))]
        [MapperIgnoreTarget(nameof(HybridMapTenantDto.TenantName))]
        [MapperIgnoreTarget(nameof(HybridMapTenantDto.Code))]
        [MapperIgnoreTarget(nameof(HybridMapTenantDto.MapCode))]
        public override partial void Map(Tenant source, HybridMapTenantDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MapTenantToHybridMapTenantDtoMapper : MapperBase<MapTenant, HybridMapTenantDto>
    {
        public override partial HybridMapTenantDto Map(MapTenant source);
        public override partial void Map(MapTenant source, HybridMapTenantDto destination);
    }
}
