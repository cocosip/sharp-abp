using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityModel;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace IdentitySample.Controllers
{
    public class HomeController : AbpController
    {
        private readonly IExtensionIdentityModelAuthenticationService _identityModelAuthenticationService;
        public HomeController(IExtensionIdentityModelAuthenticationService identityModelAuthenticationService)
        {
            _identityModelAuthenticationService = identityModelAuthenticationService;
        }

        public ActionResult Index()
        {
            return Redirect("~/swagger");
        }

        public async Task<ActionResult> UserToken()
        {
            var token1 = await _identityModelAuthenticationService.GetUserAccessTokenAsync("admin", "1q2w3E*", "Client1");
            var token2 = await _identityModelAuthenticationService.GetExternalCredentialsAccessTokenAsync("wechat", "1234567890", "Client2");
            return Content($"[{token1}]-------</br>[{token2}]");
        }


    }
}
