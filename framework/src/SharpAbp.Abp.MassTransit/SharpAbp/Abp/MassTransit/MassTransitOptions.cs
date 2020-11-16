using MassTransit.ExtensionsDependencyInjectionIntegration;
using System;

namespace SharpAbp.Abp.MassTransit
{
    public class MassTransitOptions
    {
        public Action<IServiceCollectionBusConfigurator> BusConfigurator { get; set; }

        public Action<IServiceCollectionMediatorConfigurator> MediatorConfigurator { get; set; }
    }
}
