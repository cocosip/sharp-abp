using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public interface ILoadBalancer
    {
        string Type { get; }

        string Service { get; }

        Task<MicroService> Lease(string tag = "", CancellationToken cancellationToken = default);
    }
}
