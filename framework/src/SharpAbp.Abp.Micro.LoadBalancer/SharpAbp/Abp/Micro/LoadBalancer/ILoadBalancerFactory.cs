using JetBrains.Annotations;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public interface ILoadBalancerFactory
    {
        ILoadBalancer Get([NotNull] string service);
    }
}
