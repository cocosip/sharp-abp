using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.TenantGroupManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class TenantGroupToTenantGroupDtoMapper : MapperBase<TenantGroup, TenantGroupDto>
    {
        public override partial TenantGroupDto Map(TenantGroup source);
        public override partial void Map(TenantGroup source, TenantGroupDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class TenantGroupConnectionStringToTenantGroupConnectionStringDtoMapper : MapperBase<TenantGroupConnectionString, TenantGroupConnectionStringDto>
    {
        public override partial TenantGroupConnectionStringDto Map(TenantGroupConnectionString source);
        public override partial void Map(TenantGroupConnectionString source, TenantGroupConnectionStringDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class TenantGroupTenantToTenantGroupTenantDtoMapper : MapperBase<TenantGroupTenant, TenantGroupTenantDto>
    {
        public override partial TenantGroupTenantDto Map(TenantGroupTenant source);
        public override partial void Map(TenantGroupTenant source, TenantGroupTenantDto destination);
    }
}
