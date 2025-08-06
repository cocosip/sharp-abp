using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using SharpAbp.Abp.MassTransit.TestImplementations;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Test module for MassTransit that replaces real message queue services with test implementations
    /// </summary>
    [DependsOn(
        typeof(AbpMassTransitModule), // Only depend on the core module, not the specific queue modules
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
      )]
    public class AbpMassTransitTestModule: AbpModule
    {
        /// <summary>
        /// Configure services for testing
        /// </summary>
        /// <param name="context">The service configuration context</param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            // Replace real message queue services with test implementations
            services.AddTransient<IKafkaProduceService, TestKafkaProduceService>();
            services.AddTransient<IRabbitMqProduceService, TestRabbitMqProduceService>();
            services.AddTransient<IActiveMqProduceService, TestActiveMqProduceService>();
        }
    }
}
