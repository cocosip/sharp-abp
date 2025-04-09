using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
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
    [Dependency(ServiceLifetime.Transient, ReplaceServices = false, TryRegister = true)]
    [ExposeServices(typeof(TokenAuthHandler), IncludeDefaults = true, IncludeSelf = true)]
    public class TokenAuthHandler : IAbpTransformSecurityMiddlewareHandler, ITransientDependency
    {
        private readonly ISecurityEncryptionService _securityEncryptionService;
        public TokenAuthHandler(ISecurityEncryptionService securityEncryptionService)
        {
            _securityEncryptionService = securityEncryptionService;
        }

        public async Task HandleAsync(HttpContext context, string identifier, CancellationToken cancellationToken = default)
        {
            //判断是否为登录这个方法
            if (context.Request.Method == HttpMethods.Post && context.Request.HasFormContentType && context.Request.Path.StartsWithSegments("/connect/token", StringComparison.OrdinalIgnoreCase))
            {
                if (identifier.IsNullOrWhiteSpace())
                {
                    throw new AbpException("TokenAuth handler identifier is null or empty");
                }

                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync(cancellationToken);
                if (!string.IsNullOrEmpty(body))
                {
                    var query = QueryHelpers.ParseQuery(body);
                    var form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in query)
                    {
                        if (item.Key.Equals("password", StringComparison.OrdinalIgnoreCase))
                        {
                            var p = WebUtility.UrlDecode(item.Value.ToString());
                            //明文密码
                            var plainPassword = await _securityEncryptionService.DecryptAsync(p, identifier, cancellationToken);
                            form.Add(item.Key, plainPassword);
                        }
                        else
                        {
                            form.Add(item.Key, item.Value.ToString());
                        }
                    }

                    context.Request.Body = await new FormUrlEncodedContent(form).ReadAsStreamAsync(cancellationToken);
                    context.Request.ContentLength = context.Request.Body.Length;
                }
            }
        }

    }
}
