using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityModel;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.IdentityModel;

namespace IdentityModelSample.Controllers
{
    public class HomeController : AbpController
    {
        private readonly ISharpAbpIdentityModelAuthenticationService _sharpAbpIdentityModelAuthenticationService;
        public HomeController(ISharpAbpIdentityModelAuthenticationService sharpAbpIdentityModelAuthenticationService)
        {
            _sharpAbpIdentityModelAuthenticationService = sharpAbpIdentityModelAuthenticationService;
        }
        public ActionResult Index()
        {
            return Redirect("~/swagger");
        }


        [HttpGet]
        public async Task<string> UserToken()
        {
            var token1 = await _sharpAbpIdentityModelAuthenticationService.GetUserAccessTokenAsync("admin", "1q2w3E*", "Client1");

            var token2 = await _sharpAbpIdentityModelAuthenticationService.GetExternalCredentialsAccessTokenAsync("wechat", "1234567890", "Client2");

            return $"[{token1}]-------[{token2}]";
        }
    }
}
