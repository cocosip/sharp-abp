using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore.Controllers
{
    [Route("api/SecurityKey")]
    [IgnoreAntiforgeryToken]
    public class SecurityKeyController : AbpController
    {
        private readonly ISecurityKeyManager _securityKeyManager;
        public SecurityKeyController(ISecurityKeyManager securityKeyManager)
        {
            _securityKeyManager = securityKeyManager;
        }

        /// <summary>
        /// 获取公钥 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Public")]
        [EnableRateLimiting(TransformSecurityRatelimitNames.SecurityKeyRateLimiting)]
        public async Task<SecurityPublicKeyDto> GetSecurityKeyAsync(string bizType)
        {
            var securityKey = await _securityKeyManager.GenerateAsync(bizType);
            var pub = new SecurityPublicKeyDto()
            {
                UniqueId = securityKey.UniqueId,
                BizType = securityKey.BizType,
                KeyType = securityKey.KeyType,
                PublicKey = securityKey.PublicKey
            };
            return pub;
        }
    }
}
