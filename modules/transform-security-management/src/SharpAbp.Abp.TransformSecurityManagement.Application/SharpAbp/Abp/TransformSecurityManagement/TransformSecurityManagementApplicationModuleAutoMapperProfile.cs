using AutoMapper;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class TransformSecurityManagementApplicationModuleAutoMapperProfile : Profile
    {
        public TransformSecurityManagementApplicationModuleAutoMapperProfile()
        {
            CreateMap<SecurityCredentialInfo, SecurityCredentialInfoDto>();
        }
    }
}
