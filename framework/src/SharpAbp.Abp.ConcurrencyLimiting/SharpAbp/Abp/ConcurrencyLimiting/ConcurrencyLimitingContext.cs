using System.Collections.Generic;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingContext
    {
        public string? Parameter
        {
            get; set;
        }

        public IDictionary<string, object?> ExtraProperties { get; } = new Dictionary<string, object?>();
    }

}
