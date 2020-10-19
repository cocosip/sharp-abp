using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public interface ILoadBalancerCreator
    {
        string Type { get; }

        ILoadBalancer Create(LoadBalancerConfiguration configuration, [NotNull] string service);

    }
}
