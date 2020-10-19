namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public interface ILoadBalancerConfigurationProvider
    {
        /// <summary>
        /// Get loadbalancer configuration by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        LoadBalancerConfiguration Get(string name);
    }
}
