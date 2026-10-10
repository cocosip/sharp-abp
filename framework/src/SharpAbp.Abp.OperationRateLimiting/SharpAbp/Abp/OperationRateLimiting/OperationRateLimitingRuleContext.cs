

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingRuleContext
    {
        public OperationRateLimitingRuleDefinition Definition { get; }
        public OperationRateLimitingContext Context { get; }
        public string StoreKey { get; }

        public OperationRateLimitingRuleContext(OperationRateLimitingRuleDefinition definition,
            OperationRateLimitingContext context, string storeKey)
        {
            Definition = definition;
            Context = context;
            StoreKey = storeKey;
        }
    }
}
