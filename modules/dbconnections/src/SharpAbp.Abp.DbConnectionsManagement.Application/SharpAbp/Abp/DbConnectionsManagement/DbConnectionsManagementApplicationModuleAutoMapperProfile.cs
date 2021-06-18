using AutoMapper;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DbConnectionsManagementApplicationModuleAutoMapperProfile : Profile
    {
        public DbConnectionsManagementApplicationModuleAutoMapperProfile()
        {
            CreateMap<DatabaseConnectionInfo, DatabaseConnectionInfoDto>();
        }
    }
}
