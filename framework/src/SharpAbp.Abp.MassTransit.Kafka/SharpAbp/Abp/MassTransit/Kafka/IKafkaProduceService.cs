using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.MassTransit.Kafka
{
    public interface IKafkaProduceService
    {
        /// <summary>
        /// Produce message
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ProduceAsync<TKey, TValue>(TKey key, TValue value, CancellationToken cancellationToken = default) where TValue : class;

        /// <summary>
        /// Produce string key message
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ProduceStringKeyAsync<TValue>(TValue value, CancellationToken cancellationToken = default) where TValue : class;
    }
}
