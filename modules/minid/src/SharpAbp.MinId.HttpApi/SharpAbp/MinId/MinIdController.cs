using SharpAbp.MinId.Localization;
using Volo.Abp.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Authorization;

namespace SharpAbp.MinId
{
    public abstract class MinIdController : AbpController
    {
        protected MinIdController()
        {
            LocalizationResource = typeof(MinIdResource);
        }

        protected virtual string GetBearerToken(string authorization)
        {
            if (HttpContext?.Request.Query.ContainsKey("token") == true ||
                !AuthenticationHeaderValue.TryParse(authorization, out var header) ||
                !string.Equals(header.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(header.Parameter))
            {
                throw new AbpAuthorizationException("A Bearer token in the Authorization header is required.");
            }

            return header.Parameter;
        }

        protected virtual MinIdResponse<T> UnexpectedFailure<T>(Exception exception)
        {
            var reference = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
            var logger = LazyServiceProvider != null ? Logger :
                HttpContext?.RequestServices?.GetService<ILogger<MinIdController>>() ?? NullLogger<MinIdController>.Instance;
            // Exception messages may contain credentials; keep type/stack diagnostics with the reference.
            logger.LogError("MinId operation failed. Reference: {Reference}; ExceptionType: {ExceptionType}; StackTrace: {StackTrace}",
                reference, exception.GetBaseException().GetType().FullName, exception.StackTrace);
            return MinIdResponse.Failed<T>($"An unexpected error occurred. Reference: {reference}.");
        }
    }
}
