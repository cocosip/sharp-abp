using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.IdentityServer
{
    public abstract class IdentityServerAppServiceBase : ApplicationService
    {
        protected IdentityServerAppServiceBase()
        {
            ObjectMapperContext = typeof(AbpIdentityServerApplicationModule);
            //LocalizationResource = typeof(FileStoringPolicyResource);
        }
    }
}
