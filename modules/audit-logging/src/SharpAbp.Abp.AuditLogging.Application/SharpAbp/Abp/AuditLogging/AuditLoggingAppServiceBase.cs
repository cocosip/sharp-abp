using SharpAbp.Abp.AuditLogging.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.AuditLogging
{
    public abstract class AuditLoggingAppServiceBase : ApplicationService
    {
        protected AuditLoggingAppServiceBase()
        {
            ObjectMapperContext = typeof(AuditLoggingApplicationModule);
            LocalizationResource = typeof(AuditLoggingResource);
        }
    }
}
