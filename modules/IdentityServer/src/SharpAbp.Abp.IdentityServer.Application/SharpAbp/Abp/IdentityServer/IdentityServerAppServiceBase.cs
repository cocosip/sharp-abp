﻿using SharpAbp.Abp.IdentityServer.Localization;
using Volo.Abp.Application.Services;

namespace SharpAbp.Abp.IdentityServer
{
    public abstract class IdentityServerAppServiceBase : ApplicationService
    {
        protected IdentityServerAppServiceBase()
        {
            ObjectMapperContext = typeof(IdentityServerApplicationModule);
            LocalizationResource = typeof(IdentityServerResource);
        }
    }
}
