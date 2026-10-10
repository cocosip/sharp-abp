using System;
using Microsoft.Extensions.Localization;
using SharpAbp.Abp.OperationRateLimiting.Localization;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingFormatter : IOperationRateLimitingFormatter, ITransientDependency
    {
        protected IStringLocalizer<OperationRateLimitingResource> Localizer { get; }

        public OperationRateLimitingFormatter(IStringLocalizer<OperationRateLimitingResource> localizer)
        {
            Localizer = localizer;
        }

        public virtual string Format(TimeSpan duration)
        {
            return Localizer["DurationSeconds", Math.Ceiling(Math.Max(0, duration.TotalSeconds))];
        }
    }
}
