using SharpAbp.Abp.TransformSecurityManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public abstract class TransformSecurityManagementController : AbpController
    {
        public TransformSecurityManagementController()
        {
            LocalizationResource = typeof(AbpTransformSecurityManagementResource);
        }
    }
}
