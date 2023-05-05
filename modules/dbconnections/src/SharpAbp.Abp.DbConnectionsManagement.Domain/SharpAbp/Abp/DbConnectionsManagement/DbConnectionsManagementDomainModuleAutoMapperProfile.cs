using AutoMapper;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DbConnectionsManagementDomainModuleAutoMapperProfile : Profile
    {
        public DbConnectionsManagementDomainModuleAutoMapperProfile()
        {
            CreateMap<DatabaseConnectionInfo, DatabaseConnectionInfoEto>();
        }
    }
}
