using AutoMapper;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyManagementApplicationAutoMapperProfile : Profile
    {
        public MapTenancyManagementApplicationAutoMapperProfile()
        {
            CreateMap<MapTenant, MapTenantDto>();
            CreateMap<CreateMapTenantDto, MapTenant>();
            CreateMap<UpdateMapTenantDto, MapTenant>();

            CreateMap<Tenant, HybridMapTenantDto>();
            CreateMap<MapTenant, HybridMapTenantDto>();
        }
    }
}