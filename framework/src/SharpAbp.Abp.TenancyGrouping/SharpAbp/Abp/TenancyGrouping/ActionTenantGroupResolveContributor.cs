using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class ActionTenantGroupResolveContributor : TenantGroupResolveContributorBase
    {
        public const string ContributorName = "Action";

        public override string Name => ContributorName;

        private readonly Action<ITenantGroupResolveContext> _resolveAction;

        public ActionTenantGroupResolveContributor([NotNull] Action<ITenantGroupResolveContext> resolveAction)
        {
            Check.NotNull(resolveAction, nameof(resolveAction));

            _resolveAction = resolveAction;
        }

        public override Task ResolveAsync(ITenantGroupResolveContext context)
        {
            _resolveAction(context);
            return Task.CompletedTask;
        }
    }
}
