using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class FixedWindowOperationRateLimitingRule : IOperationRateLimitingRule, ITransientDependency
    {
        protected IOperationRateLimitingStore Store { get; }
        protected IClock Clock { get; }

        public FixedWindowOperationRateLimitingRule(IOperationRateLimitingStore store, IClock clock)
        {
            Store = store;
            Clock = clock;
        }

        public virtual async Task<OperationRateLimitingRuleStatus> GetStatusAsync(OperationRateLimitingRuleContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var state = context.Definition.MaxCount == 0 ? new OperationRateLimitingStoreState() :
                await Store.GetAsync(context.StoreKey, cancellationToken).ConfigureAwait(false);
            return CreateStatus(context.Definition, state);
        }

        public virtual async Task<OperationRateLimitingRuleResult> ConsumeAsync(OperationRateLimitingRuleContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (context.Definition.MaxCount == 0)
            {
                return new OperationRateLimitingRuleResult(false,
                    CreateStatus(context.Definition, new OperationRateLimitingStoreState()));
            }
            var result = await Store.IncrementAsync(context.StoreKey, context.Definition.Duration,
                context.Definition.MaxCount, cancellationToken).ConfigureAwait(false);
            return new OperationRateLimitingRuleResult(result.IsAccepted, CreateStatus(context.Definition, result.State));
        }

        public virtual Task ResetAsync(OperationRateLimitingRuleContext context, CancellationToken cancellationToken = default)
        {
            return Store.ResetAsync(context.StoreKey, cancellationToken);
        }

        protected virtual OperationRateLimitingRuleStatus CreateStatus(OperationRateLimitingRuleDefinition definition,
            OperationRateLimitingStoreState state)
        {
            var remaining = Math.Max(0, definition.MaxCount - state.CurrentCount);
            TimeSpan? retryAfter = null;
            if (remaining == 0 && definition.MaxCount > 0 && state.WindowEndUtc.HasValue)
            {
                var now = Clock.Now;
                var utcNow = now.Kind == DateTimeKind.Utc ? now : now.ToUniversalTime();
                retryAfter = state.WindowEndUtc.Value > utcNow ? state.WindowEndUtc.Value - utcNow : TimeSpan.Zero;
            }
            return new OperationRateLimitingRuleStatus(definition.Name, remaining > 0, definition.MaxCount,
                state.CurrentCount, remaining, retryAfter, state.WindowEndUtc);
        }
    }
}
