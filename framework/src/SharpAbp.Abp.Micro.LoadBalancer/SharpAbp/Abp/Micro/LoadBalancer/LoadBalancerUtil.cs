using SharpAbp.Abp.Micro.Discovery;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public static class LoadBalancerUtil
    {

        /// <summary>
        /// Parse string to WeightServiceHostAndPort list, exp: 127.0.0.1:100-3,127.0.0.2:101-2,127.0.0.3:102-5
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<WeightServiceHostAndPort> ParseWeightHostAndPorts(string value)
        {
            var weightHostAndPorts = new List<WeightServiceHostAndPort>();
            if (value.IsNullOrWhiteSpace())
            {
                return weightHostAndPorts;
            }

            foreach (var item in value.Split(','))
            {
                var valueArray = item.Split('-');
                var weight = int.Parse(valueArray[1]);
                var hostAndPortArray = valueArray[0].Split(':');

                var serviceHostAndPort = new ServiceHostAndPort(hostAndPortArray[0], int.Parse(hostAndPortArray[1]));

                var weightServiceHostAndPort = new WeightServiceHostAndPort(serviceHostAndPort, weight);
                weightHostAndPorts.Add(weightServiceHostAndPort);
            }

            return weightHostAndPorts;
        }
    }
}
