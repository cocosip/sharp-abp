using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public static class OperationRateLimitingCheckerExtensions
    {
        public static Task CheckAsync(this IOperationRateLimitingChecker checker, string policyName,
            OperationRateLimitingArguments arguments, CancellationToken cancellationToken = default)
        {
            return checker.CheckAsync(policyName, new OperationRateLimitingContext(arguments), cancellationToken);
        }

        public static Task<bool> IsAllowedAsync(this IOperationRateLimitingChecker checker, string policyName,
            OperationRateLimitingArguments arguments, CancellationToken cancellationToken = default)
        {
            return checker.IsAllowedAsync(policyName, new OperationRateLimitingContext(arguments), cancellationToken);
        }

        public static Task<OperationRateLimitingStatus> GetStatusAsync(this IOperationRateLimitingChecker checker,
            string policyName, OperationRateLimitingArguments arguments, CancellationToken cancellationToken = default)
        {
            return checker.GetStatusAsync(policyName, new OperationRateLimitingContext(arguments), cancellationToken);
        }

        public static Task ResetAsync(this IOperationRateLimitingChecker checker, string policyName,
            OperationRateLimitingArguments arguments, CancellationToken cancellationToken = default)
        {
            return checker.ResetAsync(policyName, new OperationRateLimitingContext(arguments), cancellationToken);
        }

        public static Task CheckAsync(this IOperationRateLimitingChecker checker, string policyName,
            string parameter, CancellationToken cancellationToken = default)
        {
            return checker.CheckAsync(policyName, new OperationRateLimitingContext { Parameter = parameter }, cancellationToken);
        }

        public static Task<bool> IsAllowedAsync(this IOperationRateLimitingChecker checker, string policyName,
            string parameter, CancellationToken cancellationToken = default)
        {
            return checker.IsAllowedAsync(policyName, new OperationRateLimitingContext { Parameter = parameter }, cancellationToken);
        }

        public static Task<OperationRateLimitingStatus> GetStatusAsync(this IOperationRateLimitingChecker checker,
            string policyName, string parameter, CancellationToken cancellationToken = default)
        {
            return checker.GetStatusAsync(policyName, new OperationRateLimitingContext { Parameter = parameter }, cancellationToken);
        }

        public static Task ResetAsync(this IOperationRateLimitingChecker checker, string policyName,
            string parameter, CancellationToken cancellationToken = default)
        {
            return checker.ResetAsync(policyName, new OperationRateLimitingContext { Parameter = parameter }, cancellationToken);
        }
    }
}
