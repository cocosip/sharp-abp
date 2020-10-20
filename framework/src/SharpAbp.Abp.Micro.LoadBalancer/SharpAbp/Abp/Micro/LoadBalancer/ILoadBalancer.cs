using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public interface ILoadBalancer
    {
        string BalancerType { get; }

        string Service { get; }

        Task<MicroService> Lease(string tag = "", CancellationToken cancellationToken = default);
    }
}
