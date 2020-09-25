namespace SharpAbp.Abp.Micro.Discovery
{
    public static class ServiceDiscoveryFactoryExtensions
    {
        /// <summary>
        /// Get a named
        /// </summary>
        /// <typeparam name="TContainer"></typeparam>
        /// <param name="serviceDiscoveryFactory"></param>
        /// <returns></returns>
        public static IServiceDiscoverer Create<T>(
            this IServiceDiscoveryFactory serviceDiscoveryFactory
        )
        {
            return serviceDiscoveryFactory.Create(
                ServiceNameAttribute.GetServiceName<T>()
            );
        }
    }
}
