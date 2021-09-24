using AutoMapper;
using Volo.Abp.AuditLogging;

namespace SharpAbp.Abp.AuditLogging
{
    public class AuditLoggingApplicationModuleAutoMapperProfile : Profile
    {
        public AuditLoggingApplicationModuleAutoMapperProfile()
        {
            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<EntityChange, EntityChangeDto>();
            CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();
            CreateMap<AuditLogAction, AuditLogActionDto>();
            CreateMap<EntityChangeWithUsername, EntityChangeWithUsernameDto>();
        }
    }
}
