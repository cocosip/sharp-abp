using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
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
        /// Provider
        /// </summary>
        public string Provider { get; set; }

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

        public List<Action<IServiceCollectionBusConfigurator>> PostConfigures { get; set; }

        public AbpMassTransitOptions()
        {
            PreConfigures = new List<Action<IServiceCollectionBusConfigurator>>();
            PostConfigures = new List<Action<IServiceCollectionBusConfigurator>>();
        }


        public AbpMassTransitOptions PreConfigure(IConfiguration configuration)
        {
            var massTransitOptions = configuration
                .GetSection("MassTransitOptions")
                .Get<AbpMassTransitOptions>();

            if (massTransitOptions != null)
            {
                Prefix = massTransitOptions.Prefix;
                Provider = massTransitOptions.Provider;
                WaitUntilStarted = massTransitOptions.WaitUntilStarted;
                StartTimeoutMilliSeconds = massTransitOptions.StartTimeoutMilliSeconds;
                StopTimeoutMilliSeconds = massTransitOptions.StopTimeoutMilliSeconds;
            }

            return this;
        }



    }
}
