using AutoMapper;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupManagementApplicationAutoMapperProfile : Profile
    {
        public TenantGroupManagementApplicationAutoMapperProfile()
        {
            CreateMap<TenantGroup, TenantGroupDto>();
            CreateMap<TenantGroupConnectionString, TenantGroupConnectionStringDto>();
            CreateMap<TenantGroupTenant, TenantGroupTenantDto>();
        }
    }
}
