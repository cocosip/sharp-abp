using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore.Controllers
{
    /// <summary>
    /// Security credential controller for retrieving encryption public keys and related operations
    /// </summary>
    [Route("api/security-credentials")]
    [IgnoreAntiforgeryToken]
    public class SecurityKeyController : AbpController
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        private readonly ILogger<SecurityKeyController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityKeyController"/> class
        /// </summary>
        /// <param name="securityCredentialManager">The security credential manager</param>
        /// <param name="logger">The logger instance</param>
        public SecurityKeyController(
            ISecurityCredentialManager securityCredentialManager,
            ILogger<SecurityKeyController> logger)
        {
            _securityCredentialManager = securityCredentialManager;
            _logger = logger;
        }

        /// <summary>
        /// Gets the public key information of security credential by business type
        /// </summary>
        /// <param name="bizType">The business type</param>
        /// <returns>The security credential public key information</returns>
        /// <exception cref="AbpException">Thrown when business type is invalid or credential generation fails</exception>
        [HttpGet]
        [Route("public")]
        [EnableRateLimiting(TransformSecurityRatelimitNames.SecurityKeyRateLimiting)]
        public async Task<SecurityCredentialPublicKeyDto> GetSecurityKeyAsync(string bizType)
        {
            try
            {
                _logger.LogDebug("Requesting security credential public key for business type: {BizType}", bizType);

                if (string.IsNullOrWhiteSpace(bizType))
                {
                    const string errorMessage = "Business type cannot be null or empty";
                    _logger.LogWarning(errorMessage);
                    throw new AbpException(errorMessage);
                }

                var credential = await _securityCredentialManager.GenerateAsync(bizType);
                
                if (credential == null)
                {
                    var errorMessage = $"Failed to generate security credential for business type: {bizType}";
                    _logger.LogError(errorMessage);
                    throw new AbpException(errorMessage);
                }

                var result = new SecurityCredentialPublicKeyDto
                {
                    Identifier = credential.Identifier,
                    BizType = credential.BizType,
                    KeyType = credential.KeyType,
                    PublicKey = credential.PublicKey
                };

                _logger.LogDebug("Successfully generated security credential public key for business type: {BizType}, Identifier: {Identifier}, KeyType: {KeyType}", 
                    bizType, credential.Identifier, credential.KeyType);

                return result;
            }
            catch (Exception ex) when (!(ex is AbpException))
            {
                _logger.LogError(ex, "Unexpected error occurred while generating security credential for business type: {BizType}", bizType);
                throw new AbpException($"An error occurred while generating security credential for business type: {bizType}", ex);
            }
        }
    }
}
