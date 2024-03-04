using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore.Controllers
{
    [Route("api/SecurityCredential")]
    [IgnoreAntiforgeryToken]
    public class SecurityKeyController : AbpController
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        public SecurityKeyController(ISecurityCredentialManager securityCredentialManager)
        {
            _securityCredentialManager = securityCredentialManager;
        }

        /// <summary>
        /// 获取公钥 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Public")]
        [EnableRateLimiting(TransformSecurityRatelimitNames.SecurityKeyRateLimiting)]
        public async Task<SecurityCredentialPublicKeyDto> GetSecurityKeyAsync(string bizType)
        {
            var credential = await _securityCredentialManager.GenerateAsync(bizType);
            var pub = new SecurityCredentialPublicKeyDto()
            {
                Identifier = credential.Identifier,
                BizType = credential.BizType,
                KeyType = credential.KeyType,
                PublicKey = credential.PublicKey
            };
            return pub;
        }
    }
}
