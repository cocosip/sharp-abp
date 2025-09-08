using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    /// <summary>
    /// Token authentication handler for processing encrypted password decryption in OAuth2 token requests
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = false, TryRegister = true)]
    [ExposeServices(typeof(TokenAuthHandler), IncludeDefaults = true, IncludeSelf = true)]
    public class TokenAuthHandler : IAbpTransformSecurityMiddlewareHandler, ITransientDependency
    {
        private readonly ISecurityEncryptionService _securityEncryptionService;
        private readonly ILogger<TokenAuthHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthHandler"/> class
        /// </summary>
        /// <param name="securityEncryptionService">The security encryption service</param>
        /// <param name="logger">The logger instance</param>
        public TokenAuthHandler(
            ISecurityEncryptionService securityEncryptionService,
            ILogger<TokenAuthHandler> logger)
        {
            _securityEncryptionService = securityEncryptionService;
            _logger = logger;
        }

        /// <summary>
        /// Handles encrypted password processing in HTTP context
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <param name="identifier">The security identifier</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleAsync(HttpContext context, string identifier, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if this is a token authentication request
                if (context.Request.Method == HttpMethods.Post && 
                    context.Request.HasFormContentType && 
                    context.Request.Path.StartsWithSegments("/connect/token", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Processing token authentication request for path: {Path}", context.Request.Path);

                    if (identifier.IsNullOrWhiteSpace())
                    {
                        const string errorMessage = "Security identifier cannot be null or empty for token authentication";
                        _logger.LogError(errorMessage);
                        throw new AbpException(errorMessage);
                    }

                    using var reader = new StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync(cancellationToken);
                    
                    if (!body.IsNullOrEmpty())
                    {
                        _logger.LogDebug("Processing form data for token authentication with identifier: {Identifier}", identifier);
                        
                        var query = QueryHelpers.ParseQuery(body);
                        var form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        
                        foreach (var item in query)
                        {
                            if (item.Key.Equals("password", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    var encryptedPassword = WebUtility.UrlDecode(item.Value.ToString());
                                    _logger.LogDebug("Decrypting password for token authentication");
                                    
                                    // Decrypt the encrypted password to plain text
                                    var plainPassword = await _securityEncryptionService.DecryptAsync(encryptedPassword, identifier, cancellationToken);
                                    form.Add(item.Key, plainPassword);
                                    
                                    _logger.LogDebug("Password decryption completed successfully");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to decrypt password for token authentication with identifier: {Identifier}", identifier);
                                    throw new AbpException("Failed to decrypt password for token authentication", ex);
                                }
                            }
                            else
                            {
                                form.Add(item.Key, item.Value.ToString());
                            }
                        }

                        context.Request.Body = await new FormUrlEncodedContent(form).ReadAsStreamAsync(cancellationToken);
                        context.Request.ContentLength = context.Request.Body.Length;
                        
                        _logger.LogDebug("Token authentication request processing completed successfully");
                    }
                    else
                    {
                        _logger.LogWarning("Token authentication request body is empty");
                    }
                }
            }
            catch (Exception ex) when (ex is not AbpException)
            {
                _logger.LogError(ex, "Unexpected error occurred while processing token authentication request");
                throw new AbpException("An error occurred while processing the token authentication request", ex);
            }
        }
    }
}
