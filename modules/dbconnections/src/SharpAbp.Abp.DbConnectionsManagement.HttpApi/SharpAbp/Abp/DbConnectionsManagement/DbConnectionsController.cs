using SharpAbp.Abp.DbConnectionsManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public abstract class DbConnectionsController : AbpController
    {
        protected DbConnectionsController()
        {
            LocalizationResource = typeof(DbConnectionsManagementResource);
        }
    }
}
