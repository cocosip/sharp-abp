using SharpAbp.Abp.IdentityServer.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerController : AbpController
    {
        public IdentityServerController()
        {
            LocalizationResource = typeof(IdentityServerResource);
        }
    }
}
