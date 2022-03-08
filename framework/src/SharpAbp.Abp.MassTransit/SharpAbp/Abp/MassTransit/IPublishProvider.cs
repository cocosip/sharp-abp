using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit
{
    public interface IPublishProvider
    {
        string Provider { get; }

        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}
