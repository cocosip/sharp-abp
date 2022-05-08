using SharpAbp.MinId.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.MinId
{
    public abstract class MinIdAppService : ApplicationService
    {
        protected MinIdAppService()
        {
            LocalizationResource = typeof(MinIdResource);
            ObjectMapperContext = typeof(MinIdApplicationModule);
        }
    }
}
