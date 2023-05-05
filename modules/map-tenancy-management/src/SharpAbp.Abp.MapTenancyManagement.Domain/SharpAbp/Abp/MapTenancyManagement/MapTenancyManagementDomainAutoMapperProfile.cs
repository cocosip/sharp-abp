using AutoMapper;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyManagementDomainAutoMapperProfile : Profile
    {
        public MapTenancyManagementDomainAutoMapperProfile()
        {
            CreateMap<MapTenant, MapTenantEto>();
        }
    }
}
