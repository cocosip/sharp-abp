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
    /// Password grant token handler for processing encrypted password decryption in OAuth2 token requests
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = false, TryRegister = true)]
    [ExposeServices(typeof(PasswordGrantTokenHandler), IncludeDefaults = true, IncludeSelf = true)]
    public class PasswordGrantTokenHandler : IAbpTransformSecurityMiddlewareHandler, ITransientDependency
    {
        private readonly ISecurityEncryptionService _securityEncryptionService;
        private readonly ILogger<PasswordGrantTokenHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGrantTokenHandler"/> class
        /// </summary>
        /// <param name="securityEncryptionService">The security encryption service</param>
        /// <param name="logger">The logger instance</param>
        public PasswordGrantTokenHandler(
            ISecurityEncryptionService securityEncryptionService,
            ILogger<PasswordGrantTokenHandler> logger)
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
                    context.Request.Path.StartsWithSegments("/connect/token", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Processing token authentication request for path: {Path}", context.Request.Path);

                    // Only process x-www-form-urlencoded content for token endpoint per OAuth2 spec
                    var isUrlEncoded = context.Request.ContentType?.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) == true;
                    if (!isUrlEncoded)
                    {
                        _logger.LogDebug("Content-Type is not 'application/x-www-form-urlencoded' (value: {ContentType}); skipping decryption.", context.Request.ContentType);
                        // Ensure downstream can still read original body
                        context.Request.EnableBuffering();
                        context.Request.Body.Position = 0;
                        return;
                    }

                    // Enable buffering so we can safely read and, if needed, reset the body stream
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
                    var body = await reader.ReadToEndAsync(cancellationToken);
                    
                    if (!body.IsNullOrEmpty())
                    {
                        _logger.LogDebug("Processing form data for token authentication with identifier: {Identifier}", identifier);
                        
                        var query = QueryHelpers.ParseQuery(body);
                        var form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        var grantType = query.TryGetValue("grant_type", out var grantTypeValues)
                            ? grantTypeValues.ToString()
                            : string.Empty;
                        var isPasswordGrant = grantType.Equals("password", StringComparison.OrdinalIgnoreCase);

                        // Only require identifier and perform decryption for password grant type
                        if (!isPasswordGrant)
                        {
                            _logger.LogDebug("grant_type is not 'password' (value: {GrantType}), skipping decryption.", grantType);
                            // Keep original body unmodified for non-password grants
                            context.Request.Body.Position = 0;
                            return;
                        }

                        // For password grant, if password is missing, skip silently
                        var hasPassword = query.TryGetValue("password", out var passwordValues) && !passwordValues.ToString().IsNullOrEmpty();
                        if (!hasPassword)
                        {
                            const string errorMessage = "'password' is required for 'password' grant type.";
                            _logger.LogError(errorMessage);
                            throw new AbpException(errorMessage);
                        }

                        // For password grant with password present, identifier must be provided
                        if (identifier.IsNullOrWhiteSpace())
                        {
                            const string errorMessage = "Security identifier is required for 'password' grant type.";
                            _logger.LogError(errorMessage);
                            throw new AbpException(errorMessage);
                        }

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
                        context.Request.ContentType = "application/x-www-form-urlencoded";
                        
                        _logger.LogDebug("Token authentication request processing completed successfully");
                    }
                    else
                    {
                        _logger.LogWarning("Token authentication request body is empty");
                        // Reset position so downstream can still read the body (even if empty)
                        context.Request.Body.Position = 0;
                        return;
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
