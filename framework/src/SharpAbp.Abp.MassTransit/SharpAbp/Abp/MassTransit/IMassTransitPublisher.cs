using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit
{
    public interface IMassTransitPublisher
    {
        /// <summary>
        /// Publish message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}
