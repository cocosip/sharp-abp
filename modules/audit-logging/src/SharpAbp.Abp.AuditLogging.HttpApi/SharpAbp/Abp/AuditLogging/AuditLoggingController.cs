using SharpAbp.Abp.AuditLogging.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.AuditLogging
{
    public abstract class AuditLoggingController : AbpController
    {
        protected AuditLoggingController()
        {
            LocalizationResource = typeof(AuditLoggingResource);
        }
    }
}
