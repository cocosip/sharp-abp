using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public interface ILoadBalancerHouse
    {
        ILoadBalancer Get([NotNull] string service);
    }
}
