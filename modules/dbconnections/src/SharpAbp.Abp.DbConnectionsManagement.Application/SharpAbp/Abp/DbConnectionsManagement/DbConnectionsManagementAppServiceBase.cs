using SharpAbp.Abp.DbConnectionsManagement.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public abstract class DbConnectionsManagementAppServiceBase : ApplicationService
    {
        protected DbConnectionsManagementAppServiceBase()
        {
            ObjectMapperContext = typeof(DbConnectionsManagementApplicationModule);
            LocalizationResource = typeof(DbConnectionsManagementResource);
        }
    }
}
