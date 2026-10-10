using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    /// <summary>Startup enablement and named synchronous partition resolvers.</summary>
    public interface IConcurrencyLimitingConfiguration
    {
        bool IsEnabled
        {
            get;
        }
        string Resolve(string name, IServiceProvider services, ConcurrencyLimitingContext context);
    }

}
