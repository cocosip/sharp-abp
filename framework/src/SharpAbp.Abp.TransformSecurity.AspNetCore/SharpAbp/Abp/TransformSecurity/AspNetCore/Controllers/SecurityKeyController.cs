using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore.Controllers
{
    /// <summary>
    /// 安全凭证控制器，用于获取加密公钥等操作
    /// </summary>
    [Route("api/security-credentials")]
    [IgnoreAntiforgeryToken]
    public class SecurityKeyController : AbpController
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="securityCredentialManager">安全凭证管理器</param>
        public SecurityKeyController(ISecurityCredentialManager securityCredentialManager)
        {
            _securityCredentialManager = securityCredentialManager;
        }

        /// <summary>
        /// 根据业务类型获取安全凭证的公钥信息
        /// </summary>
        /// <param name="bizType">业务类型</param>
        /// <returns>安全凭证公钥信息</returns>
        [HttpGet]
        [Route("public")]
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
