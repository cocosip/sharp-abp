using SharpAbp.MinId.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.MinId
{
    public abstract class MinIdController : AbpController
    {
        protected MinIdController()
        {
            LocalizationResource = typeof(MinIdResource);
        }
    }
}
