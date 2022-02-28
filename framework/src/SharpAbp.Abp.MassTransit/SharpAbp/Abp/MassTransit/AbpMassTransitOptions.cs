using MassTransit.ExtensionsDependencyInjectionIntegration;
using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.MassTransit
{
    public class AbpMassTransitOptions
    {
        /// <summary>
        /// Topic or queue prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Wait until started
        /// </summary>
        public bool WaitUntilStarted { get; set; } = true;

        /// <summary>
        /// Start timeout milliseconds
        /// </summary>
        public int StartTimeoutMilliSeconds { get; set; } = 30000;

        /// <summary>
        /// Stop timeout millisecond
        /// </summary>
        public int StopTimeoutMilliSeconds { get; set; } = 3000;
        
        public List<Action<IServiceCollectionBusConfigurator>> PreConfigures { get; set; }

        public AbpMassTransitOptions()
        {
            PreConfigures = new List<Action<IServiceCollectionBusConfigurator>>();
        }

    }
}
