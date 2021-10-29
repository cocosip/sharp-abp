using Microsoft.AspNetCore.Mvc;
using SharpAbp.Abp.IdentityModel;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace IdentityModelSample.Controllers
{
    public class HomeController : AbpController
    {
        private readonly IIdentityModelUserAuthenticationService _identityModelUserAuthenticationService;
        public HomeController(IIdentityModelUserAuthenticationService identityModelUserAuthenticationService)
        {
            _identityModelUserAuthenticationService = identityModelUserAuthenticationService;
        }
        public ActionResult Index()
        {
            return Redirect("~/swagger");
        }


        [HttpGet]
        public async Task<string> UserToken()
        {
            return await _identityModelUserAuthenticationService.GetUserAccessTokenAsync("admin", "1q2w3E*", "Client1");
        }
    }
}
