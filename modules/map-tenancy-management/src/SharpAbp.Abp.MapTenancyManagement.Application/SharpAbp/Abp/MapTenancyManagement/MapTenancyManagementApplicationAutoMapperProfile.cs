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

            CreateMap<MapTenant, HybridMapTenantDto>()
                .ForMember(dest => dest.MapTenantId, opt => opt.MapFrom(o => o.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore());
        }
    }
}